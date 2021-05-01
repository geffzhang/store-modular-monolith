using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using OnlineStore.Modules.Identity.Application.Permissions;
using OnlineStore.Modules.Identity.Application.Roles;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Domain;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.System
{
    public class DataSeeder
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionService _permissionService;

        public DataSeeder(IUserRepository userRepository, IRoleRepository roleRepository,
            IPermissionService permissionService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionService = permissionService;
        }

        public async Task SeedAllAsync(CancellationToken cancellationToken)
        {
            await SeedUsersAsync(cancellationToken);
            await SeedRolesAsync(cancellationToken);
            await SeedPermissionsAsync(cancellationToken);
        }

        private async Task SeedUsersAsync(CancellationToken cancellationToken)
        {
            if (await _userRepository.FindByNameAsync("admin") == null)
            {
                var admin = User.Of(new UserId(Guid.Parse("1eb2fa8ac6574541afdb525833dadb46")), "admin@admin.com",
                    "admin", "admin", "admin", "admin", "admin", DateTime.Now, "admin", null,
                    nameof(UserType.Administrator), true, true, new List<string> {"admin"});

                var adminUser = await _userRepository.FindByIdAsync(admin.Id.ToString());
                if (adminUser == null) await _userRepository.AddAsync(admin);
            }
        }

        private async Task SeedRolesAsync(CancellationToken cancellationToken)
        {
            foreach (var role in Role.AllRoles())
            {
                var roleExist = await _roleRepository.RoleExistsAsync(role.Name);
                if (!roleExist)
                    //create the roles and seed them to the database: Question 1
                    await _roleRepository.AddRoleAsync(Role.Of(role.Name, role.Description,
                        role.Permissions.ToArray()));
            }
        }

        private  Task SeedPermissionsAsync(CancellationToken cancellationToken)
        {
            //Register PermissionScope type itself to prevent Json serialization error due that fact that will be taken from other derived from PermissionScope type (first in registered types list) in PolymorphJsonContractResolver
            AbstractTypeFactory<PermissionScope>.RegisterType<PermissionScope>();

            _permissionService.RegisterPermissions(SecurityConstants.Permissions.AllPermissions
                .Select(x => Permission.Of(x, "admin")).ToArray());

            return Task.CompletedTask;
        }
    }
}