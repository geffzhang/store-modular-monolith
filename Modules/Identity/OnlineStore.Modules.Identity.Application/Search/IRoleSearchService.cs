using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Search.Dtos;

namespace OnlineStore.Modules.Identity.Application.Search
{
    public interface IRoleSearchService
    {
        Task<RoleSearchResult> SearchRolesAsync(RoleSearchCriteria criteria);
    }
}