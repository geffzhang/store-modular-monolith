using System.Threading.Tasks;
using Common.Domain;
using Common.Extensions;
using Common.Messaging.Events;
using Humanizer;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Common.Logging.Serilog
{
    [Decorator]
    internal sealed class LoggingEventHandlerDecorator<T> : IEventHandler<T>
        where T : class, IEvent
    {
        private readonly IEventHandler<T> _handler;
        private readonly ILogger<IEventHandler<T>> _logger;

        public LoggingEventHandlerDecorator(IEventHandler<T> handler,
            ILogger<IEventHandler<T>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task HandleAsync(T @event)
        {
            using (LogContext.PushProperty("RequestId", $"{@event.Id:N}"))
            {
                var module = @event.GetModuleName();
                var name = @event.GetType().Name.Underscore();
                _logger.LogInformation($"Handling an event: '{name}' ('{module}')...");
                await _handler.HandleAsync(@event);
                _logger.LogInformation($"Completed handling an event: '{name}' ('{module}').");
            }
        }
    }
}