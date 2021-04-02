using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Contexts;
using Common.Messaging.Outbox;
using Common.Utils;
using Microsoft.Extensions.Logging;

namespace Common.Messaging.Transport.InMemory
{
    internal class InMemoryMessageBroker : ITransport
    {
        private readonly IContext _context;
        private readonly ILogger<InMemoryMessageBroker> _logger;
        private readonly IAsyncMessageDispatcher _messageDispatcher;
        private readonly MessagingOptions _messagingOptions;
        private readonly IModuleClient _moduleClient;
        private readonly IOutbox _outbox;

        public InMemoryMessageBroker(IModuleClient moduleClient, IAsyncMessageDispatcher messageDispatcher,
            IContext context, IOutbox outbox, MessagingOptions messagingOptions, ILogger<InMemoryMessageBroker> logger)
        {
            _moduleClient = moduleClient;
            _messageDispatcher = messageDispatcher;
            _context = context;
            _outbox = outbox;
            _messagingOptions = messagingOptions;
            _logger = logger;
        }

        public async Task PublishAsync(params IMessage[] messages)
        {
            if (messages is null) return;

            messages = messages.Where(x => x is { }).ToArray();
            if (!messages.Any()) return;

            foreach (var message in messages)
                if (message.CorrelationId == Guid.Empty)
                    message.CorrelationId = _context.CorrelationId;

            if (_outbox.Enabled)
            {
                _logger.LogInformation("Messages will be saved to the outbox...");
                await _outbox.SaveAsync(messages);
                return;
            }

            foreach (var message in messages)
            {
                var name = message.GetType().Name.Underscore();
                _logger.LogInformation($"Publishing a message: '{name}' with ID: '{message.Id:N}'...");

                if (_messagingOptions.UseBackgroundDispatcher)
                {
                    await _messageDispatcher.PublishAsync(message);
                    continue;
                }

                await _moduleClient.SendAsync(message);
            }
        }
    }
}