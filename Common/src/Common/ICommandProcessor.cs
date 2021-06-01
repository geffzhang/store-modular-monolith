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
        Task PublishDomainEventAsync(params IDomainEvent[] events);
        Task PublishMessageAsync(params IMessage[] messages);
    }
}