using System;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OnlineStore.Modules.Identity.Application.Permissions.Services;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Application.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Repositories;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Search;
using OnlineStore.Modules.Identity.Infrastructure.Triggers;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration configuration,
            Action<Authorization.AuthorizationOptions> setupAction = null)
        {
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("OnlineStore"));
                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict();

                options.UseTriggers(triggerOptions =>
                {
                    triggerOptions.AddTrigger<AuditTrigger>();
                });
            });


            services.AddTransient<IUserNameResolver, HttpContextUserResolver>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IRoleSearchService, RoleSearchService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

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

            services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            return services;
        }
    }
}