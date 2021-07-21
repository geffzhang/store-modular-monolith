using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Domain;
using Common.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using Serilog.Context;

namespace Diagnostics.Messaging
{
    [Decorator]
    public class OTelDomainEventTracingDecorator<T> : IDomainEventHandler<T> where T : class, IDomainEvent
    {
        private readonly IDomainEventHandler<T> _handler;
        private readonly ILogger<IDomainEventHandler<T>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly ActivitySource ActivitySource = new(OTeMessagingOptions.OTelDomainEventHandlerName);

        public OTelDomainEventTracingDecorator(IDomainEventHandler<T> handler, ILogger<IDomainEventHandler<T>> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _handler = handler;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(T domainEvent)
        {
            var traceId = Activity.Current?.TraceId.ToString() ?? _httpContextAccessor?.HttpContext?.TraceIdentifier;
            const string prefix = nameof(OTelDomainEventTracingDecorator<T>);
            var handlerName = $"{typeof(T).Name}Handler";
            var module = domainEvent.GetModuleName();

            using (LogContext.PushProperty("DomainEventId", $"{domainEvent.Id:N}"))
            {
                _logger.LogInformation(
                    "[{Prefix}:{HandlerName}] Handle {X-RequestData} request with TraceId={TraceId} and ModuleName={ModuleName}",
                    prefix, handlerName, typeof(T).Name, traceId, module);

                using var activity =
                    ActivitySource.StartActivity($"{OTeMessagingOptions.OTelDomainEventHandlerName}.{handlerName}",
                        ActivityKind.Server);

                activity?.AddEvent(new ActivityEvent(handlerName))
                    ?.AddTag("params.request.name", typeof(T).Name)
                    ?.AddTag("params.response.name", typeof(T).Name);

                try
                {
                    await _handler.HandleAsync(domainEvent);
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
}