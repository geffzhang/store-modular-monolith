using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging.Events
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event);
    }
}