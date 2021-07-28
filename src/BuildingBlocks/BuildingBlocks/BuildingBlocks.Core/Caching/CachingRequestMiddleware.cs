using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Caching
{
    /// <summary>
    /// Cqrs request Caching middleware
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class CachingRequestMiddleware<TRequest, TResponse> : IRequestMiddleware<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<CachingRequestMiddleware<TRequest, TResponse>> _logger;
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly int defaultCacheExpirationInHours = 1;
        private readonly IEnumerable<ICachePolicy<TRequest, TResponse>> _cachePolicies;


        public CachingRequestMiddleware(IEasyCachingProviderFactory cachingFactory,
            ILogger<CachingRequestMiddleware<TRequest, TResponse>> logger,
            IEnumerable<ICachePolicy<TRequest, TResponse>> cachePolicies)
        {
            _logger = logger;
            _cachingProvider = cachingFactory.GetCachingProvider("mem");
            _cachePolicies = cachePolicies;
        }

        public async Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken,
            HandleRequestDelegate<TRequest, TResponse> next)
        {
            // if (request is ICacheableRequest cacheable)
            // {
            //     if (string.IsNullOrEmpty(cacheable.CacheKey))
            //     {
            //         throw new ArgumentNullException(nameof(cacheable.CacheKey));
            //     }
            //
            //     var returnValue = await _cachingProvider.GetAsync<TResponse>(cacheable.CacheKey);
            //
            //     if (returnValue != null)
            //     {
            //         _logger.LogInformation(
            //             $"Returned value from cache | {typeof(TRequest).Name} with key {cacheable.CacheKey}");
            //         return returnValue.Value;
            //     }
            //
            //     var response = await next(request, cancellationToken);
            //     var time = cacheable.ExpirationOptions?.AbsoluteExpiration ??
            //                DateTimeOffset.Now.AddHours(defaultCacheExpirationInHours);
            //     await _cachingProvider.SetAsync(cacheable.CacheKey, response, time.Offset);
            //     _logger.LogInformation($"Added to cache | {typeof(TRequest).Name} with key {cacheable.CacheKey}");
            //
            //     return response;
            // }
            //
            // return await next(request, cancellationToken);

            var cachePolicy = _cachePolicies.FirstOrDefault();
            if (cachePolicy == null)
            {
                // No cache policy found, so just continue through the pipeline
                return await next(request, cancellationToken);
            }

            var cacheKey = cachePolicy.GetCacheKey(request);
            var cachedResponse = await _cachingProvider.GetAsync<TResponse>(cacheKey);
            if (cachedResponse != null)
            {
                _logger.LogDebug($"Response retrieved {typeof(TRequest).FullName} from cache. CacheKey: {cacheKey}");
                return cachedResponse.Value;
            }

            var response = await next(request, cancellationToken);
            _logger.LogDebug($"Caching response for {typeof(TRequest).FullName} with cache key: {cacheKey}");

            var time = cachePolicy.AbsoluteExpirationRelativeToNow ??
                       DateTimeOffset.Now.AddHours(defaultCacheExpirationInHours);
            await _cachingProvider.SetAsync(cacheKey, response, time.Offset);

            return response;
        }
    }
}