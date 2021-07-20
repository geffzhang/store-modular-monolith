using System.Threading.Tasks;
using Common.Core.Domain;
using Common.Core.Messaging;
using Common.Core.Messaging.Commands;
using Common.Core.Messaging.Events;

namespace Common.Core
{
    public interface ICommandProcessor
    {
        /// <summary>
        /// Sending our in-process command to command dispatcher to handling by in process command handlers
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="T"></typeparam>
        Task SendCommandAsync<T>(T command) where T : class, ICommand;

        /// <summary>
        /// Sending our in-process domain event to domain-event dispatcher to handling by in process domain event handlers
        /// </summary>
        /// <param name="domainEvents"></param>
        Task PublishDomainEventAsync(params IDomainEvent[] domainEvents);

        /// <summary>
        /// Sending our in-process domain event notification to domain-event notification dispatcher to handling by in process domain event notification handlers
        /// </summary>
        /// <param name="events"></param>
        Task PublishDomainEventNotificationAsync(params IDomainEventNotification[] events);

        /// <summary>
        /// Publish our out-of-process message or integration event to bus to handling by subscribers
        /// </summary>
        /// <param name="message"></param>
        Task PublishMessageAsync<T>(T message) where T : class, IMessage;

        /// <summary>
        /// Sending our in-process event to event dispatcher to handling by in process event handlers
        /// </summary>
        /// <param name="event"></param>
        /// <typeparam name="T"></typeparam>
        Task SendEventAsync<T>(T @event) where T : class, IEvent;
    }
}