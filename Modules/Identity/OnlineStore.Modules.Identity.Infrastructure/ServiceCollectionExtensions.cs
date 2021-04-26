using System;
using Common.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OnlineStore.Modules.Identity.Infrastructure.Authorization;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Search;

namespace OnlineStore.Modules.Identity.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            Action<AuthorizationOptions> setupAction = null)
        {
            // services.AddTransient<ISecurityRepository, SecurityRepository>();
            // services.AddTransient<Func<ISecurityRepository>>(provider =>
            //     () => provider.CreateScope().ServiceProvider.GetService<ISecurityRepository>());


            // services.AddScoped<IUserApiKeyService, UserApiKeyService>();
            // services.AddScoped<IUserApiKeySearchService, UserApiKeySearchService>();

            services.AddScoped<IUserNameResolver, HttpContextUserResolver>();
            services.AddSingleton<IPermissionService, PermissionService>();
            services.AddScoped<IRoleSearchService, RoleSearchService>();
            //Register as singleton because this abstraction can be used as dependency in singleton services
            services.AddSingleton<IUserSearchService>(provider =>
                new UserSearchService(provider.CreateScope().ServiceProvider
                    .GetService<Func<UserManager<ApplicationUser>>>()));

            //Identity dependencies override
            services.TryAddScoped<RoleManager<ApplicationRole>, CustomRoleManager>();
            services.TryAddScoped<UserManager<ApplicationUser>, CustomUserManager>();
            services.AddSingleton<Func<UserManager<ApplicationUser>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>());
            //Use custom ClaimsPrincipalFactory to add system roles claims for user principal
            services.TryAddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();

            if (setupAction != null) services.Configure(setupAction);

            // services.AddSingleton(provider =>
            //     new UserApiKeyActualizeEventHandler(provider.CreateScope().ServiceProvider
            //         .GetService<IUserApiKeyService>()));

            return services;
        }
    }
}