using System.Threading.Tasks;

namespace Common.Domain.Dispatching
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(params IDomainEvent[] events);
    }
}