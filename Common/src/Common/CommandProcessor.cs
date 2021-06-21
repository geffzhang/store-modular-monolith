using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Domain;
using Common.Domain.Dispatching;
using Common.Messaging.Commands;
using Common.Messaging.Events;
using Common.Messaging.Transport;

namespace Common
{
    internal sealed class CommandProcessor : ICommandProcessor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IDomainEventNotificationDispatcher _domainEventNotificationDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ITransport _transport;

        public CommandProcessor(IServiceProvider serviceProvider,
            IDomainEventDispatcher domainEventDispatcher,
            IDomainEventNotificationDispatcher domainEventNotificationDispatcher,
            ICommandDispatcher commandDispatcher,
            ITransport transport)
        {
            _serviceProvider = serviceProvider;
            _domainEventDispatcher = domainEventDispatcher;
            _domainEventNotificationDispatcher = domainEventNotificationDispatcher;
            _commandDispatcher = commandDispatcher;
            _transport = transport;
        }

        public async Task PublishDomainEventAsync(params IDomainEvent[] domainEvents)
        {
            if (domainEvents is null || !domainEvents.Any())
                return;

            // also will publish domain event notification internally
            await _domainEventDispatcher.DispatchAsync(domainEvents);
        }

        public async Task PublishDomainEventNotificationAsync(params IDomainEventNotification[] events)
        {
            if (events is null || !events.Any())
                return;
            
            await _domainEventNotificationDispatcher.DispatchAsync(events);
        }


        public async Task SendCommandAsync<T>(T command) where T : class, ICommand
        {
            if (command is null)
                return;

            await _commandDispatcher.SendAsync(command);
        }

        public async Task PublishMessageAsync(params IIntegrationEvent[] messages)
        {
            if (messages is null || !messages.Any())
                return;
            
            await _transport.PublishAsync(messages);
        }
    }
}