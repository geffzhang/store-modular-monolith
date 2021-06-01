using System.Threading.Tasks;
using Common.Domain;
using Common.Utils.Extensions;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Common.Logging.Serilog
{
    public class LoggingNotificationEventHandlerDecorator
    {

    }

    internal sealed class LoggingNotificationEventHandlerDecorator<T> : IDomainNotificationEventHandler<T>
        where T : class, IDomainNotificationEvent
    {
        private readonly IDomainNotificationEventHandler<T> _handler;
        private readonly ILogger<IDomainNotificationEventHandler<T>> _logger;

        public LoggingNotificationEventHandlerDecorator(IDomainNotificationEventHandler<T> handler,
            ILogger<IDomainNotificationEventHandler<T>> logger)
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