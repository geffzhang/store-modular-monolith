using Microsoft.Extensions.Caching.Memory;

namespace Common.Caching.Caching
{
    public interface IPlatformMemoryCache : IMemoryCache
    {
        MemoryCacheEntryOptions GetDefaultCacheEntryOptions();
    }
}
