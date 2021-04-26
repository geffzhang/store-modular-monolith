using System.Threading.Tasks;

namespace Common.Identity.Search
{
    public interface IUserSearchService
    {
        Task<UserSearchResult> SearchUsersAsync(UserSearchCriteria criteria);

    }
}
