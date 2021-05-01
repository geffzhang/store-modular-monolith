using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Serialization;
using Common.Messaging.Transport;
using Common.Modules;
using Common.Utils.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Common.Messaging.Outbox.Mongo
{
    internal sealed class MongoOutbox : IOutbox
    {
        private readonly string _collectionName;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoOutbox> _logger;
        private readonly IModuleClient _moduleClient;
        private readonly IAsyncMessageDispatcher _messageDispatcher;
        private readonly string[] _modules;
        private readonly bool _useBackgroundDispatcher;

        public MongoOutbox(IMongoDatabase database, IModuleRegistry moduleRegistry, OutboxOptions outboxOptions,
            MessagingOptions messagingOptions, IModuleClient moduleClient, IAsyncMessageDispatcher messageDispatcher,
            IMessageSerializer messageSerializer, ILogger<MongoOutbox> logger)
        {
            _database = database;
            _moduleClient = moduleClient;
            _messageDispatcher = messageDispatcher;
            _messageSerializer = messageSerializer;
            _logger = logger;
            Enabled = outboxOptions.Enabled;
            _modules = moduleRegistry.Modules.ToArray();
            _useBackgroundDispatcher = messagingOptions.UseBackgroundDispatcher;
            _collectionName = string.IsNullOrWhiteSpace(outboxOptions.CollectionName)
                ? "outbox"
                : outboxOptions.CollectionName;
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
            await _database.GetCollection<OutboxMessage>($"{module}-module.{_collectionName}")
                .InsertManyAsync(outboxMessages);
            _logger.LogInformation($"Saved {outboxMessages.Length} messages to the outbox ('{module}').");
        }

        public Task PublishUnsentAsync()
        {
            return Task.WhenAll(_modules.Select(PublishUnsentAsync));
        }

        private async Task PublishUnsentAsync(string module)
        {
            var collection = _database.GetCollection<OutboxMessage>($"{module}-module.{_collectionName}");
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
                await collection.ReplaceOneAsync(x => x.Id == outboxMessage.Id, outboxMessage);
                _logger.LogInformation($"Published a message: '{name}' with ID: '{message.Id} (outbox)'.");
            }
        }
    }
}