using System.Threading.Tasks;

namespace Common.Identity.Search
{
    public interface IRoleSearchService
    {
        Task<RoleSearchResult> SearchRolesAsync(RoleSearchCriteria criteria);
    }
}
