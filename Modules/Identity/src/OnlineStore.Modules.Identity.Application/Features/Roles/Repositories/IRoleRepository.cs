using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Features.Roles.Dtos;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Features.Roles.Repositories
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsAsync(string roleName);
        Task<CreateRoleResponse> AddRoleAsync(Role role);
    }
}