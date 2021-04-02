using System.Threading.Tasks;

namespace Common.Messaging.Events
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : class, IIntegrationEvent
    {
        Task HandleAsync(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}