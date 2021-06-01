using System.Threading.Tasks;

namespace Common.Domain
{
    public class DomainEventsDispatcherNotificationHandlerDecorator<T> : IDomainEventHandler<T>
        where T : IDomainEvent
    {
        private readonly IDomainEventHandler<T> _decorated;
        private readonly IDomainEventDispatcher _domainEventsDispatcher;

        public DomainEventsDispatcherNotificationHandlerDecorator(
            IDomainEventDispatcher domainEventsDispatcher,
            IDomainEventHandler<T> decorated)
        {
            _domainEventsDispatcher = domainEventsDispatcher;
            _decorated = decorated;
        }

        public async Task HandleAsync(T message)
        {
            await _decorated.HandleAsync(message);
            await _domainEventsDispatcher.DispatchAsync();
        }
    }
}