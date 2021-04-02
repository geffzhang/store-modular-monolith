using System.Threading.Tasks;
using Common.Domain;
using Common.Messaging;
using Common.Messaging.Commands;
using Common.Messaging.Events;

namespace Common
{
    public interface ICommandProcessor
    {
        Task SendCommandAsync<T>(T command) where T : class, ICommand;
        Task PublishIntegrationEventAsync<T>(T integrationEvent) where T : class, IIntegrationEvent;
        Task PublishDomainEventAsync<T>(T domainEvent) where T : class, IDomainEvent;

        Task PublishDomainNotificationEventAsync<T>(T domainNotificationEvent)
            where T : class, IDomainNotificationEvent;

        Task PublishMessageAsync<T>(T message) where T : class, IMessage;
    }
}