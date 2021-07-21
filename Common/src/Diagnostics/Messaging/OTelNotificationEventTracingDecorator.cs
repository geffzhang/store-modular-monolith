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
    public class OTelNotificationEventTracingDecorator<T> : IDomainEventNotificationHandler<T>
        where T : class, IDomainEventNotification
    {
        private readonly IDomainEventNotificationHandler<T> _handler;
        private readonly ILogger<IDomainEventNotificationHandler<T>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly ActivitySource ActivitySource = new(OTeMessagingOptions.OTelDomainEventNotificationHandlerName);

        public OTelNotificationEventTracingDecorator(IDomainEventNotificationHandler<T> handler, ILogger<IDomainEventNotificationHandler<T>> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _handler = handler;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(T domainEventNotification)
        {
            var traceId = Activity.Current?.TraceId.ToString() ?? _httpContextAccessor?.HttpContext?.TraceIdentifier;
            const string prefix = nameof(OTelNotificationEventTracingDecorator<T>);
            var handlerName = $"{typeof(T).Name}Handler";
            var module = domainEventNotification.GetModuleName();

            using (LogContext.PushProperty("DomainEventNotificationId", $"{domainEventNotification.Id:N}"))
            {
                _logger.LogInformation(
                    "[{Prefix}:{HandlerName}] Handle {X-RequestData} request with TraceId={TraceId} and ModuleName={ModuleName}",
                    prefix, handlerName, typeof(T).Name, traceId, module);

                using var activity =
                    ActivitySource.StartActivity($"{OTeMessagingOptions.OTelDomainEventNotificationHandlerName}.{handlerName}",
                        ActivityKind.Server);

                activity?.AddEvent(new ActivityEvent(handlerName))
                    ?.AddTag("params.request.name", typeof(T).Name)
                    ?.AddTag("params.response.name", typeof(T).Name);

                try
                {
                    await _handler.HandleAsync(domainEventNotification);
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