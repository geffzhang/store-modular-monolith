using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Core.Utils.Reflection;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Permissions.Services;
using OnlineStore.Modules.Identity.Application.System;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.System
{
    public class DataSeeder : IDataSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPermissionService _permissionService;

        public DataSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
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
            var user = await _userManager.FindByNameAsync("admin");
            if (user == null)
            {
                var admin = new ApplicationUser
                {
                    Id = "4073f0f0-855a-48e6-9168-d4e20f1d2839",
                    IsAdministrator = true,
                    Name = "admin",
                    FirstName = "admin",
                    LastName = "admin",
                    UserName = "admin",
                    PasswordExpired = true,
                    Email = "admin@admin.com",
                    IsActive = true,
                    UserType = UserType.Administrator.ToString(),
                    Roles = new List<ApplicationRole>()
                    {
                        Role.Admin.ToApplicationRole()
                    }
                };

                admin.PasswordHash = _userManager.PasswordHasher.HashPassword(admin, "admin");

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