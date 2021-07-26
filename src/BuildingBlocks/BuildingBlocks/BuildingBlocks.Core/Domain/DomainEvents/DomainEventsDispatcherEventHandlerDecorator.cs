// using System.Threading.Tasks;
// using BuildingBlocks.Cqrs.Events;
//
// namespace BuildingBlocks.Core.Domain.DomainEvents
// {
//     [Decorator]
//     public class DomainEventsDispatcherEventHandlerDecorator<T> : IEventHandler<T>
//         where T : IEvent
//     {
//         private readonly IEventHandler<T> _decorated;
//         private readonly IDomainEventDispatcher _domainEventsDispatcher;
//
//         public DomainEventsDispatcherEventHandlerDecorator(
//             IDomainEventDispatcher domainEventsDispatcher,
//             IEventHandler<T> decorated)
//         {
//             _domainEventsDispatcher = domainEventsDispatcher;
//             _decorated = decorated;
//         }
//
//         public async Task HandleAsync(T message)
//         {
//             await _decorated.HandleAsync(message);
//             await _domainEventsDispatcher.DispatchAsync();
//         }
//     }
// }