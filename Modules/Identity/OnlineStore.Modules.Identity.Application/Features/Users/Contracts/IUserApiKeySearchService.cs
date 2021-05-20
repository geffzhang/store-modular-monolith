using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.Contracts
{
    public interface IUserApiKeySearchService
    {
        Task<UserApiKeySearchResponse> SearchUserApiKeysAsync(UserApiKeySearchCriteriaDto criteria);
    }
}