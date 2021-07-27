using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Logging.Serilog
{
    public class CqrsRequestLoggingMiddleware<TRequest, TResponse> : IRequestMiddleware<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly ILogger<CqrsRequestLoggingMiddleware<TRequest, TResponse>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CqrsRequestLoggingMiddleware(ILogger<CqrsRequestLoggingMiddleware<TRequest, TResponse>> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken,
            HandleRequestDelegate<TRequest, TResponse> next)
        {
            const string prefix = nameof(CqrsRequestLoggingMiddleware<TRequest, TResponse>);

            _logger.LogInformation("[{Prefix}] Handle request={X-RequestData} and response={X-ResponseData}",
                prefix, typeof(TRequest).Name, typeof(TResponse).Name);

            var timer = new Stopwatch();
            timer.Start();

            var response = await next(request, cancellationToken);

            timer.Stop();
            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
            {
                _logger.LogWarning("[{Perf-Possible}] The request {X-RequestData} took {TimeTaken} seconds.",
                    prefix, typeof(TRequest).Name, timeTaken.Seconds);
            }

            _logger.LogInformation("[{Prefix}] Handled {X-RequestData}", prefix, typeof(TRequest).Name);
            return response;
        }
    }
}