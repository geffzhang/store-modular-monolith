using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Domain.Users.Types;

namespace OnlineStore.Modules.Identity.Application.Users.Contracts
{
    public interface IUserApiKeyService
    {
        Task<UserApiKey> GetApiKeyByKeyAsync(string apiKey);
        Task<UserApiKey[]> GetAllUserApiKeysAsync(string userId);

        Task<UserApiKey> GetApiKeyByIdAsync(string id);
        Task<UserApiKey[]> GetApiKeysByIdsAsync(string[] ids);

        Task<UserApiKey[]> SaveApiKeysAsync(UserApiKey[] apiKeys);
        Task DeleteApiKeysAsync(string[] ids);
    }
}
