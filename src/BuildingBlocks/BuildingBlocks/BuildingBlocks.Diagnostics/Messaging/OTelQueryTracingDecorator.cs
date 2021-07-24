using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Diagnostics.Messaging
{
    public class OTelQueryTracingDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;
        private readonly ILogger<IQueryHandler<TQuery, TResult>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly ActivitySource ActivitySource =
            new(OTeMessagingOptions.OTelQueryHandlerName);

        public OTelQueryTracingDecorator(IQueryHandler<TQuery, TResult> handler,
            ILogger<IQueryHandler<TQuery, TResult>> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _handler = handler;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResult> HandleAsync(TQuery query)
        {
            var traceId = Activity.Current?.TraceId.ToString() ?? _httpContextAccessor?.HttpContext?.TraceIdentifier;
            const string prefix = nameof(OTelQueryTracingDecorator<TQuery, TResult>);
            var handlerName = $"{typeof(TQuery).Name}Handler";
            var module = query.GetModuleName();

            _logger.LogInformation(
                "[{Prefix}:{HandlerName}] Handle {X-RequestData} request with TraceId={TraceId} and ModuleName={ModuleName}",
                prefix, handlerName, typeof(TQuery).Name, traceId, module);

            using var activity =
                ActivitySource.StartActivity(
                    $"{OTeMessagingOptions.OTelQueryHandlerName}.{handlerName}",
                    ActivityKind.Server);

            activity?.AddEvent(new ActivityEvent(handlerName))
                ?.AddTag("params.request.name", typeof(TQuery).Name)
                ?.AddTag("params.response.name", typeof(TQuery).Name);

            try
            {
                return await _handler.HandleAsync(query);
            }
            catch (Exception ex)
            {
                activity.SetStatus(Status.Error.WithDescription(ex.Message));
                activity.RecordException(ex);

                _logger.LogError(ex, "[{Prefix}:{HandlerName}] {ErrorMessage}", prefix,
                    handlerName, ex.Message);

                throw;
            }
        }
    }
}