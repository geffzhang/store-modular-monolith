using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Permissions;
using OnlineStore.Modules.Identity.Application.Permissions.Services;
using OnlineStore.Modules.Identity.Application.Roles;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Role;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Domain.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Mappings;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.System
{
    public class DataSeeder
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

        public async Task SeedAllAsync(CancellationToken cancellationToken)
        {
            await SeedRolesAsync(cancellationToken);
            await SeedUsersAsync(cancellationToken);
            await SeedPermissions(cancellationToken);
        }

        private async Task SeedUsersAsync(CancellationToken cancellationToken)
        {
            if (await _userManager.FindByNameAsync("admin") == null)
            {
                var admin = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    IsAdministrator = true,
                    UserName = "admin",
                    PasswordExpired = true,
                    Email = "admin@admin.com"
                };

                admin.PasswordHash = _userManager.PasswordHasher.HashPassword(admin, "admin");

                var adminUser = await _userManager.FindByIdAsync(admin.Id);
                if (adminUser == null)
                {
                    await _userManager.CreateAsync(admin);
                }
            }
        }

        private async Task SeedRolesAsync(CancellationToken cancellationToken)
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

        private Task SeedPermissions(CancellationToken cancellationToken)
        {
            AbstractTypeFactory<PermissionScope>.RegisterType<PermissionScope>();

            _permissionService.RegisterPermissions(Identity.Domain.SecurityConstants.Permissions.AllPermissions
                .Select(x => Permission.Of(x, "online-store")).ToArray());

            return Task.CompletedTask;
        }
    }
}