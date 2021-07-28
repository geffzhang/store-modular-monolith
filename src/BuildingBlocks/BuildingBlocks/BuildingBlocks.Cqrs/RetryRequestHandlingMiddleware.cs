using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace BuildingBlocks.Cqrs
{
    public class RetryRequestHandlingMiddleware<TRequest, TResponse> : IRequestMiddleware<TRequest, TResponse> where
        TRequest : IRequest<TResponse>
    {
        private readonly IAsyncPolicy _retryPolicy;

        public RetryRequestHandlingMiddleware()
        {
            _retryPolicy = Policy.Handle<ArgumentOutOfRangeException>()
                .WaitAndRetryAsync(3,
                    i => TimeSpan.FromSeconds(i));
        }

        public Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken,
            HandleRequestDelegate<TRequest, TResponse> next)
        {
            return _retryPolicy.ExecuteAsync(() => next(request, cancellationToken));
        }
    }
}