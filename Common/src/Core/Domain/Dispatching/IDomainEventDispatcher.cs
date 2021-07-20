using System.Threading.Tasks;

namespace Common.Core.Domain.Dispatching
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(params IDomainEvent[] events);
    }
}