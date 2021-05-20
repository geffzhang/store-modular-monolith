using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Features.Roles.Dtos;

namespace OnlineStore.Modules.Identity.Application.Features.Roles.Services
{
    public interface IRoleSearchService
    {
        Task<RoleSearchResult> SearchRolesAsync(RoleSearchCriteria criteria);
    }
}