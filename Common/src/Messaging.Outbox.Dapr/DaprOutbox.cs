using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Domain;
using Common.Core.Domain.Dispatching;
using Common.Core.Extensions;
using Common.Core.Messaging.Outbox;
using Common.Core.Messaging.Serialization;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace Common.Messaging.Outbox.Dapr
{
    public class DaprOutbox : IOutbox
    {
        private readonly OutboxOptions _options;
        private readonly DaprClient _daprClient;
        private readonly ILogger<DaprOutbox> _logger;
        private readonly IDomainNotificationsMapper _domainNotificationsMapper;
        private readonly IMessageSerializer _messageSerializer;
        private readonly ICommandProcessor _commandProcessor;
        // private readonly string[] _modules;

        public DaprOutbox(OutboxOptions options, 
            DaprClient daprClient, 
            ILogger<DaprOutbox> logger, 
            IDomainNotificationsMapper domainNotificationsMapper, 
            IMessageSerializer messageSerializer, 
            ICommandProcessor commandProcessor)
        {
            _options = options;
            _daprClient = daprClient;
            _logger = logger;
            _domainNotificationsMapper = domainNotificationsMapper;
            _messageSerializer = messageSerializer;
            _commandProcessor = commandProcessor;
            Enabled = options.Enabled;
            // _modules = moduleRegistry.Modules.ToArray();
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

            var events = await _daprClient.GetStateEntryAsync<List<OutboxMessage>>("statestore", _options.CollectionName,
                cancellationToken: cancellationToken);

            events.Value ??= new List<OutboxMessage>();

            events.Value.AddRange(outboxMessages);

            var module = outboxMessages[0].GetModuleName();

            await events.SaveAsync(cancellationToken: cancellationToken);
            _logger.LogInformation($"Saved {outboxMessages.Count} messages to the outbox ('{module}').");
        }


        public async Task<IEnumerable<OutboxMessage>> GetAllOutboxMessages(string moduleName = default)
        {
            var events = await _daprClient.GetStateEntryAsync<List<OutboxMessage>>("statestore", _options.CollectionName);
            var messages = events.Value
                .Where(x => moduleName.IsNullOrEmpty() || x.ModuleName == moduleName)
                .ToList();

            return messages;
        }

        public async Task PublishUnsentAsync()
        {
            var events = await _daprClient.GetStateEntryAsync<List<OutboxMessage>>("statestore", _options.CollectionName);
            var unsentMessages = events.Value
                .Where(x => x.SentAt == null)
                .ToList();

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

            await events.SaveAsync().ConfigureAwait(false);
        }

        public Task CleanProcessedAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
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