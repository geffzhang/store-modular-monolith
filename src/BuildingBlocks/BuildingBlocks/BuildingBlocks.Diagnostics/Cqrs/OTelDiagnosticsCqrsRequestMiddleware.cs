using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Diagnostics.Cqrs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Diagnostics.Messaging
{
    public class OTelDiagnosticsCqrsRequestMiddleware<TRequest, TResponse> : IRequestMiddleware<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<OTelDiagnosticsCqrsRequestMiddleware<TRequest, TResponse>> _logger;
        private static readonly ActivitySource CqrsActivitySource = new(OTelCqrsOptions.OTelCqrsRequestName);

        public OTelDiagnosticsCqrsRequestMiddleware(IHttpContextAccessor httpContextAccessor,
            ILogger<OTelDiagnosticsCqrsRequestMiddleware<TRequest, TResponse>> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken,
            HandleRequestDelegate<TRequest, TResponse> next)
        {
            var traceId = Activity.Current?.TraceId.ToString() ?? _httpContextAccessor?.HttpContext?.TraceIdentifier;
            const string prefix = nameof(OTelDiagnosticsCqrsRequestMiddleware<TRequest, TResponse>);
            var handlerName = $"{typeof(TRequest).Name}Handler"; // by convention

            _logger.LogInformation(
                "[{Prefix}:{HandlerName}] Handle {X-RequestData} request with TraceId={TraceId}",
                prefix, handlerName, typeof(TRequest).Name, traceId);


            using var activity = CqrsActivitySource.StartActivity($"{OTelCqrsOptions.OTelCqrsRequestName}.{handlerName}",
                ActivityKind.Server);

            activity?.AddEvent(new ActivityEvent(handlerName))
                ?.AddTag("params.request.name", typeof(TRequest).Name)
                ?.AddTag("params.response.name", typeof(TResponse).Name);

            try
            {
                return await next(request, cancellationToken);
            }
            catch (Exception ex)
            {
                activity.SetStatus(OpenTelemetry.Trace.Status.Error.WithDescription(ex.Message));
                activity.RecordException(ex);

                _logger.LogError(ex, "[{Prefix}:{HandlerName}] {ErrorMessage}", prefix,
                    handlerName, ex.Message);

                throw;
            }
        }
    }
}