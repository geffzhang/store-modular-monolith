using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Serialization;
using Common.Messaging.Transport;
using Common.Modules;
using Common.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Messaging.Outbox.EFCore
{
    public class EFCoreOutbox<TContext> : IOutbox where TContext : DbContext
    {
        private readonly IMessageSerializer _messageSerializer;
        private readonly TContext _dbContext;
        private readonly IAsyncMessageDispatcher _messageDispatcher;
        private readonly IModuleClient _moduleClient;
        private readonly ILogger<EFCoreOutbox<TContext>> _logger;
        private readonly string _collectionName;
        private readonly string[] _modules;
        private readonly bool _useBackgroundDispatcher;

        public EFCoreOutbox(IMessageSerializer messageSerializer, TContext dbContext, OutboxOptions options,
            IModuleRegistry moduleRegistry, MessagingOptions messagingOptions, IAsyncMessageDispatcher messageDispatcher,
            IModuleClient moduleClient, ILogger<EFCoreOutbox<TContext>> logger)
        {
            _messageSerializer = messageSerializer;
            _dbContext = dbContext;
            _messageDispatcher = messageDispatcher;
            _moduleClient = moduleClient;
            _logger = logger;
            Enabled = options.Enabled;
            _modules = moduleRegistry.Modules.ToArray();
            _useBackgroundDispatcher = messagingOptions.UseBackgroundDispatcher;
            _collectionName = string.IsNullOrWhiteSpace(options.CollectionName)
                ? "outbox"
                : options.CollectionName;
        }

        public bool Enabled { get; }

        public async Task SaveAsync(params IMessage[] messages)
        {
            if (!Enabled)
            {
                _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved.");
                return;
            }

            if (messages is null || !messages.Any())
            {
                _logger.LogWarning("No messages have been provided to be saved to the outbox.");
                return;
            }

            var outboxMessages = messages.Where(x => x is { })
                .Select(x => new OutboxMessage
                {
                    Id = x.Id,
                    CorrelationId = x.CorrelationId,
                    Name = x.GetType().Name.Underscore(),
                    Payload = _messageSerializer.Serialize(x),
                    Type = x.GetType().AssemblyQualifiedName,
                    ReceivedAt = DateTime.UtcNow.ToUnixTimeMilliseconds()
                }).ToArray();

            if (!outboxMessages.Any()) _logger.LogWarning("No messages have been provided to be saved to the outbox.");

            var module = messages[0].GetModuleName();
            _logger.LogInformation($"Saved {outboxMessages.Length} messages to the outbox ('{module}').");
            var outboxMessagesSet = _dbContext.Set<OutboxMessage>($"{module}-module.{_collectionName}");

            await outboxMessagesSet.AddRangeAsync(outboxMessages);
            await _dbContext.SaveChangesAsync();
        }

        public Task PublishUnsentAsync()
        {
            return Task.WhenAll(_modules.Select(PublishUnsentAsync));
        }

        private async Task PublishUnsentAsync(string module)
        {
            var collection = _dbContext.Set<OutboxMessage>($"{module}-module.{_collectionName}");
            var unsentMessages = await collection.AsQueryable()
                .Where(x => x.SentAt == null)
                .ToListAsync();

            if (!unsentMessages.Any())
            {
                _logger.LogTrace($"No unsent messages found in outbox ('{module}').");
                return;
            }

            _logger.LogInformation($"Found {unsentMessages.Count} unsent messages in outbox ('{module}'), sending...");

            foreach (var outboxMessage in unsentMessages)
            {
                var type = Type.GetType(outboxMessage.Type);
                var message = _messageSerializer.Deserialize(outboxMessage.Payload, type) as IMessage;

                if (message is null)
                {
                    _logger.LogError($"Invalid message type: {type?.Name} (cannot cast to {nameof(IMessage)}).");
                    continue;
                }

                message.Id = outboxMessage.Id;
                message.CorrelationId = outboxMessage.CorrelationId;

                var name = message.GetType().Name.Underscore();
                _logger.LogInformation($"Publishing a message: '{name}' with ID: '{message.Id}' (outbox)...");

                if (_useBackgroundDispatcher)
                    await _messageDispatcher.PublishAsync(message);
                else
                    await _moduleClient.PublishAsync(message);

                outboxMessage.SentAt = DateTime.UtcNow.ToUnixTimeMilliseconds();
                _logger.LogInformation($"Published a message: '{name}' with ID: '{message.Id} (outbox)'.");
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}