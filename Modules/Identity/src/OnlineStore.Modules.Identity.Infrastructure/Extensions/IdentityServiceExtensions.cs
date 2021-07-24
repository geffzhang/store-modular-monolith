using System;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OnlineStore.Modules.Identity.Application.Permissions.Services;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Services;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Authorization;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class IdentityServiceExtensions
    {
        private const string SectionName = "mssql";

        internal static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration configuration,
            Action<AuthorizationOptions> setupAction = null,
            string sectionName = SectionName)
        {
            services.TryAddScoped<IUserNameResolver, UserNameResolver>();
            services.TryAddScoped<IPermissionService, PermissionService>();
            // services.TryAddScoped<IRoleSearchService, RoleSearchService>();
            services.TryAddTransient<IUserRepository, UserRepository>();

            //Identity dependencies override
            services.TryAddScoped<RoleManager<ApplicationRole>, CustomRoleManager>();
            services.TryAddSingleton<Func<RoleManager<ApplicationRole>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>());


            services.TryAddScoped<UserManager<ApplicationUser>, CustomUserManager>();
            services.TryAddSingleton<Func<UserManager<ApplicationUser>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>());

            services.TryAddSingleton<Func<SignInManager<ApplicationUser>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>());

            //Use custom ClaimsPrincipalFactory to add system roles claims for user principal
            services.TryAddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();

            //Platform authorization handler for policies based on permissions
            services.AddSingleton<IAuthorizationHandler, DefaultPermissionAuthorizationHandler>();

            if (setupAction != null)
                services.Configure(setupAction);

            //some dependencies will add here if not registered before
            services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options
            services.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));
            var authorizationOptions = configuration.GetSection("Authorization").Get<AuthorizationOptions>();

            return services;
        }
    }
}