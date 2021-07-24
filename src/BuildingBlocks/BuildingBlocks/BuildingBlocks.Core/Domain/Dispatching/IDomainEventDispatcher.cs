using System.Threading.Tasks;

namespace BuildingBlocks.Core.Domain.Dispatching
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(params IDomainEvent[] events);
    }
}