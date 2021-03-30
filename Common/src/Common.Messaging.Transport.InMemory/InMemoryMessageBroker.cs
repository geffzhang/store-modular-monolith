using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Common.Messaging.Outbox;
using IInfrastructure.Contexts;
using Common.Utils;

namespace Common.Messaging.Transport.InMemory
{
    internal class InMemoryMessageBroker : ITransport
    {
        private readonly IModuleClient _moduleClient;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IContext _context;
        private readonly IOutbox _outbox;
        private readonly MessagingOptions _messagingOptions;
        private readonly ILogger<InMemoryMessageBroker> _logger;

        public InMemoryMessageBroker(IModuleClient moduleClient, ICommandProcessor commandProcessor,
            IContext context, IOutbox outbox, MessagingOptions messagingOptions, ILogger<InMemoryMessageBroker> logger)
        {
            _moduleClient = moduleClient;
            _commandProcessor = commandProcessor;
            _context = context;
            _outbox = outbox;
            _messagingOptions = messagingOptions;
            _logger = logger;
        }

        public async Task PublishAsync(params IMessage[] messages)
        {
            if (messages is null)
            {
                return;
            }

            messages = messages.Where(x => x is {}).ToArray();
            if (!messages.Any())
            {
                return;
            }

            foreach (var message in messages)
            {
                if (message.CorrelationId == Guid.Empty)
                {
                    message.CorrelationId = _context.CorrelationId;
                }
            }

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
                    await _commandProcessor.PublishMessageAsync(message);
                    continue;
                }

                await _moduleClient.SendAsync(message);
            }
        }
    }
}