using Common.Tests.Integration.Constants;
using Common.Tests.Integration.Mocks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Tests.Integration.Extensions
{
    public static class AuthServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddTestAuthentication(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(AuthConstants.Scheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services.AddAuthentication(AuthConstants.Scheme)
                .AddScheme<AuthenticationSchemeOptions, IntegrationTestAuthenticationHandler>(AuthConstants.Scheme,
                    options => { });
        }
    }
}