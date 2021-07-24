using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging.Events
{
    public interface  IEventDispatcher
    {
        Task DispatchAsync<T>(T @event) where T : class, IEvent;
    }
}