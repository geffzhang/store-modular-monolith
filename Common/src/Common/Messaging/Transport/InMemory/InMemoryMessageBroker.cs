using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Events;
using Common.Messaging.Outbox;
using Common.Modules;
using Common.Utils;
using Common.Utils.Extensions;
using Common.Web.Contexts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Messaging.Transport.InMemory
{
    internal class InMemoryMessageBroker : ITransport
    {
        private readonly IContext _context;
        private readonly ILogger<InMemoryMessageBroker> _logger;
        private readonly IIntegrationEventDispatcher _dispatcher;
        private readonly IAsyncMessageDispatcher _messageDispatcher;
        private readonly MessagingOptions _messagingOptions;

        public InMemoryMessageBroker(IAsyncMessageDispatcher messageDispatcher,
            IContext context,
            IOptions<MessagingOptions> messagingOptions,
            ILogger<InMemoryMessageBroker> logger,
            IIntegrationEventDispatcher dispatcher)
        {
            _messageDispatcher = messageDispatcher;
            _context = context;
            _messagingOptions = messagingOptions.Value;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task PublishAsync(params IIntegrationEvent[] messages)
        {
            if (messages is null) return;

            messages = messages.Where(x => x is { }).ToArray();
            if (!messages.Any()) return;

            foreach (var message in messages)
                if (message.CorrelationId == Guid.Empty)
                    message.CorrelationId = _context.CorrelationId;


            foreach (var message in messages)
            {
                var name = message.GetType().Name.Underscore();
                _logger.LogInformation($"Publishing a message: '{name}' with ID: '{message.Id:N}'...");

                if (_messagingOptions.UseBackgroundDispatcher)
                {
                    await _messageDispatcher.PublishAsync(message);
                    continue;
                }

                await PublishIntegrationEventAsync(messages);
            }
        }

        private async Task PublishIntegrationEventAsync<T>(T[] integrationEvents) where T : class, IIntegrationEvent
        {
            if (integrationEvents is null || integrationEvents.Any() == false)
                return;

            foreach (var integrationEvent in integrationEvents)
            {
                await _dispatcher.PublishAsync(integrationEvent);
            }
        }
    }
}