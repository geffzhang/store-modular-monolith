using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Extensions;
using Common.Core.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using Serilog.Context;

namespace Diagnostics.Messaging
{
    [Decorator]
    internal sealed class OTelMessageTracingDecorator<T> : IMessageHandler<T> where T : class, IMessage
    {
        private readonly IMessageHandler<T> _handler;
        private readonly ILogger<IMessageHandler<T>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly ActivitySource ActivitySource = new(OTeMessagingOptions.OTelMessageHandlerName);

        public OTelMessageTracingDecorator(IMessageHandler<T> handler, ILogger<IMessageHandler<T>> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _handler = handler;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(T message)
        {
            var traceId = Activity.Current?.TraceId.ToString() ?? _httpContextAccessor?.HttpContext?.TraceIdentifier;
            const string prefix = nameof(OTelMessageTracingDecorator<T>);
            var handlerName = $"{typeof(T).Name}Handler";
            var module = message.GetModuleName();
            var correlationId = message.CorrelationId;
            var messageId = message.Id;

            using (LogContext.PushProperty("CorrelationId", $"{correlationId:N}"))
            {
                using (LogContext.PushProperty("MessageId", $"{messageId:N}"))
                    _logger.LogInformation(
                        "[{Prefix}:{HandlerName}] Handle {X-RequestData} request with TraceId={TraceId} and ModuleName={ModuleName}",
                        prefix, handlerName, typeof(T).Name, traceId, module);

                using var activity =
                    ActivitySource.StartActivity($"{OTeMessagingOptions.OTelMessageHandlerName}.{handlerName}",
                        ActivityKind.Server);

                activity?.AddEvent(new ActivityEvent(handlerName))
                    ?.AddTag("params.request.name", typeof(T).Name)
                    ?.AddTag("params.response.name", typeof(T).Name);

                try
                {
                    await _handler.HandleAsync(message);
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