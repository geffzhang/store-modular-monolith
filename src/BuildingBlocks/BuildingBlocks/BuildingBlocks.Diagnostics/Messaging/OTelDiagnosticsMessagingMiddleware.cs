using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using Serilog.Context;

namespace BuildingBlocks.Diagnostics.Messaging
{
    public class OTelDiagnosticsMessagingMiddleware<TMessage> : IMessageMiddleware<TMessage>
        where TMessage : class, IMessage
    {
        private readonly ILogger<OTelDiagnosticsMessagingMiddleware<TMessage>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly ActivitySource MessagingActivitySource = new(OTeMessagingOptions.OTelMessagingName);

        public OTelDiagnosticsMessagingMiddleware(ILogger<OTelDiagnosticsMessagingMiddleware<TMessage>> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RunAsync(TMessage message, IMessageContext messageContext, CancellationToken cancellationToken,
            HandleMessageDelegate<TMessage> next)
        {
            var traceId = Activity.Current?.TraceId.ToString() ?? _httpContextAccessor?.HttpContext?.TraceIdentifier;
            const string prefix = nameof(OTelDiagnosticsMessagingMiddleware<TMessage>);
            var handlerName = $"{typeof(TMessage).Name}Handler";
            var module = message.GetModuleName();

            using (LogContext.PushProperty("MessageId", $"{message.Id:N}"))
            {
                _logger.LogInformation(
                    "[{Prefix}:{HandlerName}] Handle {X-MessageData} message with TraceId={TraceId} and ModuleName={ModuleName}",
                    prefix, handlerName, typeof(TMessage).Name, traceId, module);

                using var activity =
                    MessagingActivitySource.StartActivity($"{OTeMessagingOptions.OTelMessagingName}.{handlerName}",
                        ActivityKind.Server);

                activity?.AddEvent(new ActivityEvent(handlerName))
                    ?.AddTag("params.message.name", typeof(TMessage).Name);

                try
                {
                    await next(message, messageContext, cancellationToken);
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