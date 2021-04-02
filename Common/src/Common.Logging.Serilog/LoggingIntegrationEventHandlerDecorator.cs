using System.Threading.Tasks;
using Common.Messaging.Events;
using Common.Utils;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Common.Logging
{
    [Decorator]
    internal sealed class LoggingIntegrationEventHandlerDecorator<T> : IIntegrationEventHandler<T>
        where T : class, IIntegrationEvent
    {
        private readonly IIntegrationEventHandler<T> _handler;
        private readonly ILogger<IIntegrationEventHandler<T>> _logger;

        public LoggingIntegrationEventHandlerDecorator(IIntegrationEventHandler<T> handler,
            ILogger<IIntegrationEventHandler<T>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task HandleAsync(T @event)
        {
            using (LogContext.PushProperty("CorrelationId", $"{@event.CorrelationId:N}"))
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