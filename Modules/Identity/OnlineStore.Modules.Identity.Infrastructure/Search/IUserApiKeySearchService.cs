using System.Threading.Tasks;

namespace Common.Identity.Search
{
    public interface IUserApiKeySearchService
    {
        Task<UserApiKeySearchResult> SearchUserApiKeysAsync(UserApiKeySearchCriteria criteria);
    }
}
