using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Users.Dtos;

namespace OnlineStore.Modules.Identity.Application.Users.Services
{
    public interface IUserApiKeySearchService
    {
        Task<UserApiKeySearchResponse> SearchUserApiKeysAsync(UserApiKeySearchCriteriaDto criteria);
    }
}