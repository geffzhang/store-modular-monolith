using System.Threading.Tasks;
using Common.Messaging;
using Humanizer;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Common.Logging.Serilog
{
    // [Decorator]
    // internal sealed class LoggingMessageHandlerDecorator<T> : IMessageHandler<T> where T : class, IMessage
    // {
    //     private readonly IMessageHandler<T> _handler;
    //     private readonly ILogger<IMessageHandler<T>> _logger;
    //
    //     public LoggingMessageHandlerDecorator(IMessageHandler<T> handler,
    //         ILogger<IMessageHandler<T>> logger)
    //     {
    //         _handler = handler;
    //         _logger = logger;
    //     }
    //
    //     public async Task HandleAsync(T @event)
    //     {
    //         using (LogContext.PushProperty("CorrelationId", $"{@event.CorrelationId:N}"))
    //         {
    //             using (LogContext.PushProperty("RequestId", $"{@event.Id:N}"))
    //             {
    //                 var module = @event.GetModuleName();
    //                 var name = @event.GetType().Name.Underscore();
    //                 _logger.LogInformation($"Handling an integration event: '{name}' ('{module}')...");
    //                 await _handler.HandleAsync(@event);
    //                 _logger.LogInformation($"Completed handling an integration event: '{name}' ('{module}').");
    //             }
    //         }
    //     }
    // }
}