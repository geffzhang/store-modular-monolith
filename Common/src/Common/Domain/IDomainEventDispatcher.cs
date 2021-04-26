using System.Threading.Tasks;

namespace Common.Domain
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(params IDomainEvent[] events);
    }
}