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
    internal sealed class LoggingDomainEventHandlerDecorator<T> : IDomainEventHandler<T>
        where T : class, IDomainEvent
    {
        private readonly IDomainEventHandler<T> _handler;
        private readonly ILogger<IDomainEventHandler<T>> _logger;

        public LoggingDomainEventHandlerDecorator(IDomainEventHandler<T> handler,
            ILogger<IDomainEventHandler<T>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task HandleAsync(T @event)
        {
            using (LogContext.PushProperty("EventId", $"{@event.Id:N}"))
            {
                var module = @event.GetModuleName();
                var name = @event.GetType().Name.Underscore();
                _logger.LogInformation($"Handling an domain event: '{name}' ('{module}')...");
                await _handler.HandleAsync(@event);
                _logger.LogInformation($"Completed handling an domain event: '{name}' ('{module}').");
            }
        }
    }
}