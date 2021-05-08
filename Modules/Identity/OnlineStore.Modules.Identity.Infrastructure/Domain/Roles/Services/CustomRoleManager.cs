using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common;
using Common.Caching.Caching;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineStore.Modules.Identity.Application.Permissions;
using OnlineStore.Modules.Identity.Application.Permissions.Services;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Infrastructure.Caching;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Roles.Services
{
    public class CustomRoleManager : AspNetRoleManager<ApplicationRole>
    {
        private readonly IPermissionService _knownPermissions;
        private readonly IExtendedMemoryCache _memoryCache;
        private readonly MvcNewtonsoftJsonOptions _jsonOptions;

        public CustomRoleManager(
            IPermissionService knownPermissions
            , IExtendedMemoryCache memoryCache
            , IRoleStore<ApplicationRole> store
            , IEnumerable<IRoleValidator<ApplicationRole>> roleValidators
            , ILookupNormalizer keyNormalizer
            , IdentityErrorDescriber errors
            , ILogger<RoleManager<ApplicationRole>> logger
            , IHttpContextAccessor contextAccessor
            , IOptions<MvcNewtonsoftJsonOptions> jsonOptions)
            : base(store, roleValidators, keyNormalizer, errors, logger, contextAccessor)
        {
            _knownPermissions = knownPermissions;
            _memoryCache = memoryCache;
            _jsonOptions = jsonOptions.Value;
        }

        public override async Task<ApplicationRole> FindByNameAsync(string roleName)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByNameAsync", roleName);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken());
                var role = await base.FindByNameAsync(roleName);
                if (role != null)
                {
                    await LoadRolePermissionsAsync(role);
                }

                return role;
            }, cacheNullValue: false);
            return result;
        }

        public override async Task<ApplicationRole> FindByIdAsync(string roleId)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByIdAsync", roleId);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken());
                var role = await base.FindByIdAsync(roleId);
                if (role != null)
                {
                    await LoadRolePermissionsAsync(role);
                }

                return role;
            }, cacheNullValue: false);
            return result;
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationRole role)
        {
            var result = await base.CreateAsync(role);
            if (result.Succeeded && !role.Permissions.IsNullOrEmpty())
            {
                var existRole = string.IsNullOrEmpty(role.Id)
                    ? await base.FindByNameAsync(role.Name)
                    : await base.FindByIdAsync(role.Id);
                var permissionRoleClaims =
                    role.Permissions.Select(x => new Claim(SecurityConstants.Claims.PermissionClaimType, x.Name));
                foreach (var claim in permissionRoleClaims)
                {
                    //Need to use an existing tracked by EF entity in order to add permissions for role
                    await base.AddClaimAsync(existRole, claim);
                }

                SecurityCacheRegion.ExpireRegion();
            }

            return result;
        }

        public override async Task<IdentityResult> UpdateAsync(ApplicationRole updateRole)
        {
            if (updateRole == null)
            {
                throw new ArgumentNullException(nameof(updateRole));
            }

            ApplicationRole existRole = null;
            if (!string.IsNullOrEmpty(updateRole.Id))
            {
                existRole = await base.FindByIdAsync(updateRole.Id);
            }

            if (existRole == null)
            {
                existRole = await base.FindByNameAsync(updateRole.Name);
            }

            if (existRole != null)
            {
                //Need to path exists tracked by EF  entity due to already being tracked exception 
                //https://github.com/aspnet/Identity/issues/1807
                updateRole.Patch(existRole);
            }

            var result = await base.UpdateAsync(existRole);
            if (result.Succeeded && updateRole.Permissions != null)
            {
                var sourcePermissionClaims = updateRole.Permissions
                    .Select(x => x.ToClaim(_jsonOptions.SerializerSettings)).ToList();
                var targetPermissionClaims = (await GetClaimsAsync(existRole))
                    .Where(x => x.Type == SecurityConstants.Claims.PermissionClaimType).ToList();
                var comparer = AnonymousComparer.Create((Claim x) => x.Value);
                //Add
                foreach (var sourceClaim in sourcePermissionClaims.Except(targetPermissionClaims, comparer))
                {
                    await base.AddClaimAsync(existRole, sourceClaim);
                }

                //Remove
                foreach (var targetClaim in targetPermissionClaims.Except(sourcePermissionClaims, comparer).ToArray())
                {
                    await base.RemoveClaimAsync(existRole, targetClaim);
                }

                SecurityCacheRegion.ExpireRegion();
            }

            return result;
        }

        public override async Task<IdentityResult> DeleteAsync(ApplicationRole role)
        {
            var result = await base.DeleteAsync(role);
            if (result.Succeeded)
            {
                SecurityCacheRegion.ExpireRegion();
            }

            return result;
        }

        protected virtual async Task LoadRolePermissionsAsync(ApplicationRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (SupportsRoleClaims)
            {
                role.Permissions = new List<Permission>();
                //Load role claims and convert it to the permissions and assign to role
                var storedPermissions = (await GetClaimsAsync(role))
                    .Select(x => Permission.TryCreateFromClaim(x, _jsonOptions.SerializerSettings)).ToList();
                var knownPermissionsDict = _knownPermissions.GetAllPermissions().Select(x => x.Clone() as Permission)
                    .ToDictionary(x => x.Name, x => x);
                foreach (var storedPermission in storedPermissions)
                {
                    //Copy all meta information from registered to stored (for particular role) permission
                    var knownPermission = knownPermissionsDict[storedPermission.Name];
                    if (knownPermission != null)
                    {
                        knownPermission.Patch(storedPermission);
                    }

                    role.Permissions.Add(storedPermission);
                }
            }
        }
    }
}