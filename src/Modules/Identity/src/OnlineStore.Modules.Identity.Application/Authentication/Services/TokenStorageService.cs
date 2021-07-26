using System;
using System.Threading.Tasks;
using BuildingBlocks.Authentication.Jwt;
using BuildingBlocks.Web.Storage;
using EasyCaching.Core;

namespace OnlineStore.Modules.Identity.Application.Authentication.Services
{
    public class TokenStorageService : ITokenStorageService
    {
        private readonly IRequestStorage _requestStorage;

        public TokenStorageService(IRequestStorage requestStorage)
        {
            _requestStorage = requestStorage;
        }

        public async Task<JsonWebToken> GetAsync(Guid commandId)
        {
           return  await _requestStorage.Get<JsonWebToken>(GetKey(commandId));
        }

        public async Task SetAsync(Guid commandId, JsonWebToken token)
        {
            await _requestStorage.Set(GetKey(commandId), token);
        }

        private static string GetKey(Guid commandId) => $"users:tokens:{commandId}";
    }
}