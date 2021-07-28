using System;
using System.Linq;
using BuildingBlocks.Cqrs;

namespace BuildingBlocks.Core.Caching
{
    // Using C# 8.0 to provide a default interface implementation.
    // Optionally, could move this to an AbstractCachingPolicy like AbstractValidator.
    public interface ICachePolicy<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        DateTimeOffset? AbsoluteExpirationRelativeToNow { get; }

        string GetCacheKey(TRequest request)
        {
            var r = new {request};
            var props = r.request.GetType().GetProperties().Select(pi => $"{pi.Name}:{pi.GetValue(r.request, null)}");
            return $"{typeof(TRequest).FullName}{{{String.Join(",", props)}}}";
        }
    }
}