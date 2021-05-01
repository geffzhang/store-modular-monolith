using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Roles;
using OnlineStore.Modules.Identity.Application.Roles.Dtos;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles.Mappings;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleRepository(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<CreateRoleResponse> AddRoleAsync(Role role)
        {
            var appRole = role.ToApplicationRole();
            IdentityResult identityResult;
            identityResult = await _roleManager.CreateAsync(appRole);
            return new CreateRoleResponse(appRole.Name, identityResult.Succeeded,
                identityResult.Succeeded ? null : identityResult.Errors.Select(e => e.Description));
        }
    }
}