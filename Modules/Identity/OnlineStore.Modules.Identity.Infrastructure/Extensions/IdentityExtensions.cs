using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Common.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OnlineStore.Modules.Identity.Application.Permissions;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Search;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class IdentityExtensions
    {
        internal static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration configuration,
            Action<Authorization.AuthorizationOptions> setupAction = null)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            // services.AddTransient<Func<ISecurityRepository>>(provider =>
            //     () => provider.CreateScope().ServiceProvider.GetService<ISecurityRepository>());


            // services.AddScoped<IUserApiKeyService, UserApiKeyService>();
            // services.AddScoped<IUserApiKeySearchService, UserApiKeySearchService>();
            services.AddDbContext<SecurityDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("OnlineStore"));
                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict();
            });


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

            services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<SecurityDbContext>()
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

        internal static IApplicationBuilder UsePermissions(this IApplicationBuilder appBuilder)
        {
            //Register PermissionScope type itself to prevent Json serialization error due that fact that will be taken from other derived from PermissionScope type (first in registered types list) in PolymorphJsonContractResolver
            AbstractTypeFactory<PermissionScope>.RegisterType<PermissionScope>();

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionService>();
            permissionsProvider.RegisterPermissions(SecurityConstants.Permissions.AllPermissions
                .Select(x => Permission.Of(x, "admin")).ToArray());
            return appBuilder;
        }

        internal static async Task<IApplicationBuilder> UseDefaultRolesAsync(this IApplicationBuilder appBuilder)
        {
            using (var scope = appBuilder.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                //initializing custom roles
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                foreach (var role in Role.AllRoles())
                {
                    var roleExist = await roleManager.RoleExistsAsync(role.Name);
                    if (!roleExist)
                        //create the roles and seed them to the database: Question 1
                        await roleManager.CreateAsync(new ApplicationRole()
                        {
                            Description = role.Description,
                            Name = role.Name,
                            Id = role.Name,
                            Permissions = role.Permissions
                        });
                }
            }

            return appBuilder;
        }

        internal static async Task<IApplicationBuilder> UseDefaultUsersAsync(this IApplicationBuilder appBuilder)
        {
            using (var scope = appBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();


                if (await userManager.FindByNameAsync("admin") == null)
                {
                    var admin = new ApplicationUser
                    {
                        Id = "1eb2fa8ac6574541afdb525833dadb46",
                        IsAdministrator = true,
                        UserName = "admin",
                        PasswordHash = "AHQSmKnSLYrzj9vtdDWWnUXojjpmuDW2cHvWloGL9UL3TC9UCfBmbIuR2YCyg4BpNg==",
                        PasswordExpired = true,
                        Email = "admin@vc-demostore.com"
                    };

                    var adminUser = await userManager.FindByIdAsync(admin.Id);
                    if (adminUser == null)
                    {
                        await userManager.CreateAsync(admin);
                    }
                }
            }

            return appBuilder;
        }
    }
}