using System.Threading.Tasks;

namespace Common.Domain
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent message);
    }
}