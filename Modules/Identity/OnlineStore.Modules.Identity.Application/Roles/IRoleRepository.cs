using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Roles.Dtos;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Roles
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsAsync(string roleName);
        Task<CreateRoleResponse> AddRoleAsync(Role role);
    }
}