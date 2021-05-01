using System.Linq;
using System.Threading.Tasks;
using Common.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Extentions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePermissions(this IApplicationBuilder appBuilder)
        {
            //Register PermissionScope type itself to prevent Json serialization error due that fact that will be taken from other derived from PermissionScope type (first in registered types list) in PolymorphJsonContractResolver
            AbstractTypeFactory<PermissionScope>.RegisterType<PermissionScope>();

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionService>();
            permissionsProvider.RegisterPermissions(SecurityConstants.Permissions.AllPermissions
                .Select(x => Permission.Of(x, "admin")).ToArray());
            return appBuilder;
        }

        public static async Task<IApplicationBuilder> UseDefaultRolesAsync(this IApplicationBuilder appBuilder)
        {
            //initializing custom roles
            var roleManager = appBuilder.ApplicationServices.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = appBuilder.ApplicationServices.GetRequiredService<UserManager<ApplicationUser>>();

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

            return appBuilder;
        }

        public static async Task<IApplicationBuilder> UseDefaultUsersAsync(this IApplicationBuilder appBuilder)
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