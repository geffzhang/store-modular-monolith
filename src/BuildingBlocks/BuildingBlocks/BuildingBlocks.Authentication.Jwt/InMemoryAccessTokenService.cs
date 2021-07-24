using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace BuildingBlocks.Authentication.Jwt
{
    internal sealed class InMemoryAccessTokenService : IAccessTokenService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _expires;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InMemoryAccessTokenService(IMemoryCache cache,
            IHttpContextAccessor httpContextAccessor,
            JwtOptions jwtOptions)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _expires =  TimeSpan.FromMinutes(jwtOptions.ExpiryMinutes);
        }

        public Task<bool> IsCurrentActiveToken()
        {
            return IsActiveAsync(GetCurrentAsync());
        }

        public Task DeactivateCurrentAsync()
        {
            return DeactivateAsync(GetCurrentAsync());
        }

        public Task<bool> IsActiveAsync(string token)
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(_cache.Get<string>(GetKey(token))));
        }

        public Task DeactivateAsync(string token)
        {
            _cache.Set(GetKey(token), "revoked", new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _expires
            });

            return Task.CompletedTask;
        }

        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(' ').Last();
        }

        private static string GetKey(string token)
        {
            return $"blacklisted-tokens:{token}";
        }
    }
}