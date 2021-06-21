using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using Common.Domain.Dispatching;
using Common.Messaging.Serialization;
using Common.Modules;
using Common.Persistence.MSSQL;
using Common.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace Common.Messaging.Outbox.EFCore
{
    public class EFCoreOutbox<TContext> : IOutbox
        where TContext : DbContext, ISqlDbContext
    {
        private readonly ILogger<EFCoreOutbox<TContext>> _logger;

        // private readonly string[] _modules;
        private readonly IDomainNotificationsMapper _domainNotificationsMapper;
        private readonly ICommandProcessor _commandProcessor;
        private readonly TContext _dbContext;
        private readonly IMessageSerializer _messageSerializer;

        public EFCoreOutbox(IOptions<OutboxOptions> options,
            // IModuleRegistry moduleRegistry,
            ILogger<EFCoreOutbox<TContext>> logger,
            IDomainNotificationsMapper domainNotificationsMapper,
            IMessageSerializer messageSerializer,
            ICommandProcessor commandProcessor,
            TContext dbContext)
        {
            _logger = logger;
            _domainNotificationsMapper = domainNotificationsMapper;
            _messageSerializer = messageSerializer;
            _commandProcessor = commandProcessor;
            _dbContext = dbContext;
            Enabled = options.Value.Enabled;
            // _modules = moduleRegistry.Modules.ToArray();
        }

        public bool Enabled { get; }

        public async Task SaveAsync(IList<OutboxMessage> outboxMessages, CancellationToken cancellationToken)
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

            await _dbContext.OutboxMessages.AddRangeAsync(outboxMessages, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<OutboxMessage>> GetAllOutboxMessages(string moduleName = default)
        {
            var messages = await _dbContext.OutboxMessages.Where(x => moduleName.IsNullOrEmpty() || x.ModuleName == moduleName)
                .AsQueryable()
                .ToListAsync();

            return messages;
        }

        public async Task CleanProcessedAsync(CancellationToken cancellationToken = default)
        {
            var filter = _dbContext.OutboxMessages.Where(e => e.SentAt != null);
            _dbContext.OutboxMessages.RemoveRange(filter);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task PublishUnsentAsync()
        {
            var unsentMessages = await _dbContext.OutboxMessages.AsQueryable()
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
                var type = _domainNotificationsMapper.GetType(outboxMessage.Type);

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

                    _logger.LogInformation(
                        $"Published a message: '{outboxMessage.Name}' with ID: '{domainEventNotification.Id} (outbox)'.");
                }
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
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