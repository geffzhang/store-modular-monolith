using System.Threading.Tasks;

namespace Common.Identity
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
