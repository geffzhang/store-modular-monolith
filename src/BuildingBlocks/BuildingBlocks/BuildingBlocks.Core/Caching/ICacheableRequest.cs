namespace BuildingBlocks.Core.Caching
{
    public interface ICacheableRequest
    {
        string CacheKey { get; }
        ExpirationOptions ExpirationOptions { get; }
    }
}