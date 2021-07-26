using System.Threading.Tasks;

namespace BuildingBlocks.Core.Domain.DomainEvents
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(params IDomainEvent[] events);
    }
}