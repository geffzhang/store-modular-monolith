using Common.Tests.Integration.Constants;
using Common.Tests.Integration.Mocks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Common.Tests.Integration.Extensions
{
    public static class AuthServiceCollectionExtensions
    {
        //https://blog.joaograssi.com/posts/2021/asp-net-core-testing-permission-protected-api-endpoints/
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

        //https://stebet.net/mocking-jwt-tokens-in-asp-net-core-integration-tests/
        public static IServiceCollection AddAuthenticationWithFakeToken(this IServiceCollection services)
        {
            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var config = new OpenIdConnectConfiguration()
                {
                    Issuer = MockJwtTokens.Issuer
                };

                config.SigningKeys.Add(MockJwtTokens.SecurityKey);
                options.Configuration = config;
            });

            return services;
        }
    }
}