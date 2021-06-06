using System.Threading.Tasks;
using Common.Domain;

namespace Common.Messaging.Events
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event);
    }
}