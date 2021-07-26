using System;
using System.Threading.Tasks;
using EasyCaching.Core;

namespace BuildingBlocks.Web.Storage
{
    public class RequestStorage : IRequestStorage
    {
        private readonly IEasyCachingProvider _cachingProvider;

        public RequestStorage(IEasyCachingProviderFactory cachingFactory)
        {
            _cachingProvider = cachingFactory.GetCachingProvider("mem");
        }

        public async Task Set<T>(string key, T value)
        {
            await _cachingProvider.SetAsync(key, value, TimeSpan.FromSeconds(5));
        }

        public async Task<T> Get<T>(string key)
        {
            return (await _cachingProvider.GetAsync<T>(key)).Value;
        }
    }
}