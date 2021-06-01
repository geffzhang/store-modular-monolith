using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Contracts
{
    public interface IUserApiKeySearchService
    {
        Task<UserApiKeySearchResponse> SearchUserApiKeysAsync(UserApiKeySearchCriteriaDto criteria);
    }
}