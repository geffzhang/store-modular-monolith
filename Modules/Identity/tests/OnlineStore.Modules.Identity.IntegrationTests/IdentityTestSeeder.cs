using System.Linq;
using System.Threading.Tasks;
using Common.Tests.Integration.Constants;
using Common.Utils.Reflection;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Features.Permissions.Services;
using OnlineStore.Modules.Identity.Application.Features.System;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.IntegrationTests
{
    public class IdentityTestSeeder : IDataSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPermissionService _permissionService;

        public IdentityTestSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IPermissionService permissionService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionService = permissionService;
        }
        public async Task SeedAllAsync()
        {
            await SeedPermissions();
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedUsersAsync()
        {
            var user = await _userManager.FindByNameAsync(UsersConstants.AdminUser.Name);
            if (user == null)
            {
                var admin = new ApplicationUser
                {
                    Id = UsersConstants.AdminUser.UserId,
                    IsAdministrator = UsersConstants.AdminUser.IsAdministrator,
                    Name = UsersConstants.AdminUser.Name,
                    FirstName = UsersConstants.AdminUser.FirstName,
                    LastName = UsersConstants.AdminUser.LastName,
                    UserName = UsersConstants.AdminUser.UserName,
                    PasswordExpired = true,
                    Email = UsersConstants.AdminUser.UserEmail,
                    IsActive = UsersConstants.AdminUser.IsActive,
                    UserType = UsersConstants.AdminUser.UserType
                };

                admin.PasswordHash = _userManager.PasswordHasher.HashPassword(admin, UsersConstants.AdminUser.Password);

                var adminUser = await _userManager.FindByIdAsync(admin.Id);
                if (adminUser == null)
                {
                    await _userManager.CreateAsync(admin);
                }
            }
        }

        private async Task SeedRolesAsync()
        {
            foreach (var role in Role.AllRoles())
            {
                var roleExist = await _roleManager.RoleExistsAsync(role.Name);
                if (!roleExist)
                    //create the roles and seed them to the database: Question 1
                    await _roleManager.CreateAsync(new ApplicationRole()
                    {
                        Id = role.Name,
                        Description = role.Description,
                        Name = role.Name,
                        Permissions = role.Permissions
                    });
            }
        }

        private Task SeedPermissions()
        {
            TypeFactory<PermissionScope>.RegisterType<PermissionScope>();
            _permissionService.RegisterPermissions(Permission.GetAllPermissions().ToArray());

            return Task.CompletedTask;
        }
    }
}