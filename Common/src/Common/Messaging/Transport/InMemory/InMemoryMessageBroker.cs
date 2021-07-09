using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Messaging.Events;
using Common.Web.Contexts;
using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Messaging.Transport.InMemory
{
    internal class InMemoryMessageBroker : ITransport
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
        private readonly ILogger<InMemoryMessageBroker> _logger;
        private readonly IAsyncMessageDispatcher _asyncMessageDispatcher;
        private readonly IIntegrationEventDispatcher _integrationEventDispatcher;
        private readonly MessagingOptions _messagingOptions;

        public InMemoryMessageBroker(IAsyncMessageDispatcher asyncMessageDispatcher,
            IIntegrationEventDispatcher integrationEventDispatcher,
            ICorrelationContextAccessor correlationContextAccessor,
            IOptions<MessagingOptions> messagingOptions,
            ILogger<InMemoryMessageBroker> logger)
        {
            _asyncMessageDispatcher = asyncMessageDispatcher;
            _integrationEventDispatcher = integrationEventDispatcher;
            _correlationContextAccessor = correlationContextAccessor;
            _messagingOptions = messagingOptions.Value;
            _logger = logger;
        }

        public async Task PublishAsync(params IIntegrationEvent[] messages)
        {
            if (messages is null) return;

            messages = messages.Where(x => x is { }).ToArray();
            if (!messages.Any()) return;

            foreach (var message in messages)
                if (message.CorrelationId == Guid.Empty)
                    message.CorrelationId = Guid.Parse(_correlationContextAccessor.CorrelationContext.CorrelationId);

            foreach (var message in messages)
            {
                var name = message.GetType().Name.Underscore();
                _logger.LogInformation($"Publishing a message: '{name}' with ID: '{message.Id:N}'...");

                if (_messagingOptions.UseBackgroundDispatcher)
                {
                    await _asyncMessageDispatcher.PublishAsync(message);
                    continue;
                }

                await _integrationEventDispatcher.PublishAsync(message);
            }
        }
    }
}