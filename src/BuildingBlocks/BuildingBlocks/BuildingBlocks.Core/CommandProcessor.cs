using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Core.Domain.DomainEventNotifications;
using BuildingBlocks.Core.Domain.DomainEvents;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Commands;
using BuildingBlocks.Cqrs.Events;
using IPublisher = BuildingBlocks.Core.Messaging.Transport.IPublisher;

namespace BuildingBlocks.Core
{
    internal sealed class CommandProcessor : ICommandProcessor
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IDomainEventNotificationDispatcher _domainEventNotificationDispatcher;
        private readonly IMediator _mediator;
        private readonly IPublisher _publisher;

        public CommandProcessor(
            IDomainEventDispatcher domainEventDispatcher,
            IDomainEventNotificationDispatcher domainEventNotificationDispatcher,
            IMediator mediator,
            IPublisher publisher)
        {
            _domainEventDispatcher = domainEventDispatcher;
            _domainEventNotificationDispatcher = domainEventNotificationDispatcher;
            _mediator = mediator;
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

            await _mediator.Send(command);
        }

        public async Task<TResult> SendCommandAsync<T, TResult>(T command) where T : class, ICommand<TResult>
        {
            if (command is null)
                return default;

            return await _mediator.Send(command);
        }

        public async Task SendEventAsync<T>(T @event) where T : class, IEvent
        {
            if (@event is null)
                return;

            await _mediator.Send(@event);
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