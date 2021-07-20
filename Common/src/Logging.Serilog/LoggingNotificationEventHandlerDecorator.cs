using System.Threading.Tasks;
using Common.Core;
using Common.Core.Domain;
using Common.Core.Extensions;
using Humanizer;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Common.Logging.Serilog
{
    [Decorator]
    internal sealed class LoggingNotificationEventHandlerDecorator<T> : IDomainEventNotificationHandler<T>
        where T : class, IDomainEventNotification
    {
        private readonly IDomainEventNotificationHandler<T> _handler;
        private readonly ILogger<IDomainEventNotificationHandler<T>> _logger;

        public LoggingNotificationEventHandlerDecorator(IDomainEventNotificationHandler<T> handler,
            ILogger<IDomainEventNotificationHandler<T>> logger)
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
                _logger.LogInformation($"Handling an domain notification event: '{name}' ('{module}')...");
                await _handler.HandleAsync(@event);
                _logger.LogInformation($"Completed handling an domain notification event: '{name}' ('{module}').");
            }
        }
    }
}