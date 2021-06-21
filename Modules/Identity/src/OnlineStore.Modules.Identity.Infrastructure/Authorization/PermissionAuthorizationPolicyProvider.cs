using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Caching.Caching;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using OnlineStore.Modules.Identity.Application.Features.Permissions.Services;
using OnlineStore.Modules.Identity.Infrastructure.Authentication;

namespace OnlineStore.Modules.Identity.Infrastructure.Authorization
{
    /// <summary>
    /// https://www.jerriepelser.com/blog/creating-dynamic-authorization-policies-aspnet-core/
    /// </summary>
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly IPermissionService _permissionService;
        private readonly IEasyCachingProvider _cachingProvider;

        public PermissionAuthorizationPolicyProvider(
            IOptions<AuthorizationOptions> options,
            IPermissionService permissionService, IEasyCachingProviderFactory cachingFactory)
            : base(options)
        {
            _permissionService = permissionService;
            _cachingProvider = cachingFactory.GetCachingProvider("mem");
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // Check static policies first
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                var map = GetDynamicAuthorizationPoliciesFromModulesPermissions();
                map.TryGetValue(policyName, out policy);
            }

            return policy;
        }

        private Dictionary<string, AuthorizationPolicy> GetDynamicAuthorizationPoliciesFromModulesPermissions()
        {
            var cacheKey = CacheKey.With(GetType(), "GetDynamicAuthorizationPoliciesFromModulesPermissions");
            var result = _cachingProvider.Get(cacheKey, () =>
            {
                var resultLookup = new Dictionary<string, AuthorizationPolicy>();
                foreach (var permission in _permissionService.GetAllPermissions())
                {
                    resultLookup[permission.Name] = new AuthorizationPolicyBuilder()
                        .AddRequirements(new PermissionAuthorizationRequirement(permission.Name))
                        //Use the two schema (JwtBearer and ApiKey)  authentication for permission authorization policies.
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme,
                            ApiKeyAuthenticationOptions.DefaultScheme)
                        .Build();
                }

                return resultLookup;
            },TimeSpan.FromMinutes(10));
            
            return result.Value;
        }
    }
}