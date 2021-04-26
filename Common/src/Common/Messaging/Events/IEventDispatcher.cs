using System.Threading.Tasks;

namespace Common.Messaging.Events
{
    public interface IEventDispatcher
    {
        Task PublishAsync<T>(T @event) where T : class, IEvent;
    }
}