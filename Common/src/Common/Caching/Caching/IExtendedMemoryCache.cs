using Microsoft.Extensions.Caching.Memory;

namespace Common.Caching.Caching
{
    public interface IExtendedMemoryCache : IMemoryCache
    {
        MemoryCacheEntryOptions GetDefaultCacheEntryOptions();
    }
}
