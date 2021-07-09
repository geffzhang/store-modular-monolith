using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using Common.Domain.Dispatching;
using Common.Extensions;
using Common.Messaging.Serialization;
using Common.Persistence.Mongo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace Common.Messaging.Outbox.Mongo
{
    internal sealed class MongoOutbox : IOutbox
    {
        private readonly IMessageSerializer _messageSerializer;
        private readonly ILogger<MongoOutbox> _logger;
        private readonly IMongoDbContext _mongoDbContext;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IDomainNotificationsMapper _domainNotificationsMapper;

        public MongoOutbox(IMongoDbContext mongoDbContext,
            IOptions<OutboxOptions> outboxOptions,
            ICommandProcessor commandProcessor,
            IMessageSerializer messageSerializer,
            IDomainNotificationsMapper domainNotificationsMapper,
            ILogger<MongoOutbox> logger)
        {
            _mongoDbContext = mongoDbContext;
            _commandProcessor = commandProcessor;
            _messageSerializer = messageSerializer;
            _domainNotificationsMapper = domainNotificationsMapper;
            _logger = logger;
            Enabled = outboxOptions.Value.Enabled;
        }

        public bool Enabled { get; }

        public async Task SaveAsync(IList<OutboxMessage> outboxMessages, CancellationToken cancellationToken = default)
        {
            if (!Enabled)
            {
                _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved.");
                return;
            }

            if (outboxMessages is null || !outboxMessages.Any())
            {
                _logger.LogWarning("No messages have been provided to be saved to the outbox.");
                return;
            }

            var module = outboxMessages[0].GetModuleName();
            _logger.LogInformation($"Saved {outboxMessages.Count} messages to the outbox ('{module}').");

            if (_mongoDbContext.Transaction?.Session != null)
                await _mongoDbContext.OutboxMessages
                    .InsertManyAsync(_mongoDbContext.Transaction.Session, outboxMessages, null, cancellationToken)
                    .ConfigureAwait(false);
            else
                await _mongoDbContext.OutboxMessages.InsertManyAsync(outboxMessages, null, cancellationToken)
                    .ConfigureAwait(false);
        }


        public async Task<IEnumerable<OutboxMessage>> GetAllOutboxMessages(string moduleName = default)
        {
            var messages = await _mongoDbContext.OutboxMessages.AsQueryable()
                .Where(x => moduleName.IsNullOrEmpty() || x.ModuleName == moduleName)
                .ToListAsync();

            return messages;
        }

        public async Task PublishUnsentAsync()
        {
            var unsentMessages = await _mongoDbContext.OutboxMessages.AsQueryable()
                .Where(x => x.SentAt == null)
                .ToListAsync();

            if (!unsentMessages.Any())
            {
                _logger.LogTrace($"No unsent messages found in outbox.");
                return;
            }

            _logger.LogInformation($"Found {unsentMessages.Count} unsent messages in outbox, sending...");

            foreach (var outboxMessage in unsentMessages)
            {
                var type = _domainNotificationsMapper.GetType(outboxMessage.Type) ?? Type.GetType(outboxMessage.Type);

                var domainEventNotification = _messageSerializer.Deserialize(outboxMessage.Payload, type) as IDomainEventNotification;
                if (domainEventNotification is null)
                {
                    _logger.LogError($"Invalid DomainNotification type: {type?.Name} (cannot cast to {nameof(IDomainEventNotification)}).");
                    continue;
                }

                using (LogContext.Push(new OutboxMessageContextEnricher(domainEventNotification)))
                {
                    _logger.LogInformation(
                        $"Publishing a DomainNotification : '{outboxMessage.Name}' with ID: '{domainEventNotification.Id}' (outbox)...");

                    await _commandProcessor.PublishDomainEventNotificationAsync(domainEventNotification);


                    outboxMessage.SentAt = DateTime.UtcNow;
                    await _mongoDbContext.OutboxMessages.ReplaceOneAsync(x => x.Id == outboxMessage.Id, outboxMessage);
                    _logger.LogInformation(
                        $"Published a message: '{outboxMessage.Name}' with ID: '{domainEventNotification.Id} (outbox)'.");
                }
            }
        }


        public async Task CleanProcessedAsync(CancellationToken cancellationToken = default)
        {
            await _mongoDbContext.OutboxMessages.DeleteManyAsync(e => e.SentAt != null, cancellationToken)
                .ConfigureAwait(false);
        }

        private class OutboxMessageContextEnricher : ILogEventEnricher
        {
            private readonly IDomainEventNotification _notification;

            public OutboxMessageContextEnricher(IDomainEventNotification notification)
            {
                _notification = notification;
            }

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddOrUpdateProperty(new LogEventProperty("Context",
                    new ScalarValue($"OutboxMessage:{_notification.Id.ToString()}")));
            }
        }
    }
}