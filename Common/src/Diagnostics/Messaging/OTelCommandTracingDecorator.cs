using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Extensions;
using Common.Core.Messaging.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using Serilog.Context;

namespace Diagnostics.Messaging
{
    [Decorator]
    public class OTelCommandTracingDecorator<T> : ICommandHandler<T> where T : class, ICommand
    {
        private readonly ICommandHandler<T> _handler;
                 private readonly ILogger<ICommandHandler<T>> _logger;
                 private readonly IHttpContextAccessor _httpContextAccessor;
                 private static readonly ActivitySource ActivitySource = new(OTeMessagingOptions.OTelCommandHandlerName);

                 public OTelCommandTracingDecorator(ICommandHandler<T> handler, ILogger<ICommandHandler<T>> logger,
                     IHttpContextAccessor httpContextAccessor)
                 {
                     _handler = handler;
                     _logger = logger;
                     _httpContextAccessor = httpContextAccessor;
                 }

                 public async Task HandleAsync(T command)
                 {
                     var traceId = Activity.Current?.TraceId.ToString() ?? _httpContextAccessor?.HttpContext?.TraceIdentifier;
                     const string prefix = nameof(OTelCommandTracingDecorator<T>);
                     var handlerName = $"{typeof(T).Name}Handler";
                     var module = command.GetModuleName();

                     using (LogContext.PushProperty("CommandId", $"{command.Id:N}"))
                     {
                         _logger.LogInformation(
                             "[{Prefix}:{HandlerName}] Handle {X-RequestData} request with TraceId={TraceId} and ModuleName={ModuleName}",
                             prefix, handlerName, typeof(T).Name, traceId, module);

                         using var activity =
                             ActivitySource.StartActivity($"{OTeMessagingOptions.OTelCommandHandlerName}.{handlerName}",
                                 ActivityKind.Server);

                         activity?.AddEvent(new ActivityEvent(handlerName))
                             ?.AddTag("params.request.name", typeof(T).Name)
                             ?.AddTag("params.response.name", typeof(T).Name);

                         try
                         {
                             await _handler.HandleAsync(command);
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