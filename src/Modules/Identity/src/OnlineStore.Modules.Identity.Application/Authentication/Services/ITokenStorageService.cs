using System;
using System.Threading.Tasks;
using BuildingBlocks.Authentication.Jwt;

namespace OnlineStore.Modules.Identity.Application.Authentication.Services
{
    /// <summary>
    /// short term request session for tokens - 5 second
    /// </summary>
    public interface ITokenStorageService
    {
        Task SetAsync(Guid commandId, JsonWebToken token);
        Task<JsonWebToken> GetAsync(Guid commandId);
    }
}