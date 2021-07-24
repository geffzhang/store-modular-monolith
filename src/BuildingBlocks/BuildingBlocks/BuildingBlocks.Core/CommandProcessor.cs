using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.Dispatching;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Messaging.Commands;
using BuildingBlocks.Core.Messaging.Events;
using BuildingBlocks.Core.Messaging.Transport;

namespace BuildingBlocks.Core
{
    internal sealed class CommandProcessor : ICommandProcessor
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IDomainEventNotificationDispatcher _domainEventNotificationDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly IPublisher _publisher;

        public CommandProcessor(
            IDomainEventDispatcher domainEventDispatcher,
            IDomainEventNotificationDispatcher domainEventNotificationDispatcher,
            ICommandDispatcher commandDispatcher,
            IEventDispatcher eventDispatcher,
            IMessageDispatcher messageDispatcher,
            IPublisher publisher)
        {
            _domainEventDispatcher = domainEventDispatcher;
            _domainEventNotificationDispatcher = domainEventNotificationDispatcher;
            _commandDispatcher = commandDispatcher;
            _eventDispatcher = eventDispatcher;
            _messageDispatcher = messageDispatcher;
            _publisher = publisher;
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

            await _commandDispatcher.DispatchAsync(command);
        }

        public async Task SendEventAsync<T>(T @event) where T : class, IEvent
        {
            if (@event is null)
                return;

            await _eventDispatcher.DispatchAsync(@event);
        }

        public async Task PublishMessageAsync<T>(T message) where T : class, IMessage
        {
            if (message is null)
                return;
            //await _messageDispatcher.DispatchAsync(message).ConfigureAwait(false);
            await _publisher.PublishAsync(message);
        }
    }
}