using Common.Caching;
using Common.Extensions.DependencyInjection;
using Common.Messaging.Inbox.Mongo;
using Common.Messaging.Outbox.Mongo;
using Common.Persistence.Mongo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Infrastructure.Authorization;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // This custom provider allows able to use just [Authorize] instead of having to define [Authorize(AuthenticationSchemes = "Bearer")] above every API controller

            // // without this Bearer authorization will not work
            // services.AddSingleton<IAuthenticationSchemeProvider, CustomAuthenticationSchemeProvider>();
            services.AddCommon()
                .AddMongo()
                .AddMongoOutbox()
                .AddMongoInbox();

            services.AddCaching(configuration);
            services.AddIdentityServices(configuration, options => { })
                .AddScoped<IUserRepository, UserRepository>();

            // register the AuthorizationPolicyProvider which dynamically registers authorization policies for each permission defined in module manifest
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            // authorization handler for policies based on permissions
            services.AddSingleton<IAuthorizationHandler, DefaultPermissionAuthorizationHandler>();


            services.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));
            //services.Configure<PasswordOptionsExtended>(configuration.GetSection("IdentityOptions:Password"));
            services.Configure<UserOptionsExtended>(configuration.GetSection("IdentityOptions:User"));
            services.Configure<DataProtectionTokenProviderOptions>(
                configuration.GetSection("IdentityOptions:DataProtection"));

            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            // app.UseDefaultRolesAsync().GetAwaiter().GetResult();
            // app.UseDefaultUsersAsync().GetAwaiter().GetResult();
            // app.UsePermissions();

            return app;
        }
    }
}