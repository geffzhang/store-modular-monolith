using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using OnlineStore.Modules.Identity.Application.Permissions.Services;
using OnlineStore.Modules.Identity.Application.Roles.Services;
using OnlineStore.Modules.Identity.Application.Search;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Application.Users.Services;
using OnlineStore.Modules.Identity.Domain.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Authentication;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Repositories;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Services;
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
                options.UseTriggers(triggerOptions =>
                {
                    triggerOptions.AddTrigger<AuditTrigger>();
                });
            });

            services.TryAddScoped<IUserNameResolver, UserNameResolver>();
            services.TryAddScoped<IPermissionService, PermissionService>();
            services.TryAddScoped<IRoleSearchService, RoleSearchService>();
            services.TryAddTransient<IUserRepository, UserRepository>();

            //Identity dependencies override
            services.TryAddScoped<RoleManager<ApplicationRole>, CustomRoleManager>();
            services.TryAddSingleton<Func<RoleManager<ApplicationRole>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetService<RoleManager<ApplicationRole>>());


            services.TryAddScoped<UserManager<ApplicationUser>, CustomUserManager>();
            services.TryAddSingleton<Func<UserManager<ApplicationUser>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>());

            services.TryAddSingleton<Func<SignInManager<ApplicationUser>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetService<SignInManager<ApplicationUser>>());

            //Use custom ClaimsPrincipalFactory to add system roles claims for user principal
            services.TryAddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            if (setupAction != null) services.Configure(setupAction);

            //some dependencies will add here if not registered before
            services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            services.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));
            services.Configure<UserOptionsExtended>(configuration.GetSection("IdentityOptions:User"));

            return services;
        }


        internal static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            return null;
        }
    }
}