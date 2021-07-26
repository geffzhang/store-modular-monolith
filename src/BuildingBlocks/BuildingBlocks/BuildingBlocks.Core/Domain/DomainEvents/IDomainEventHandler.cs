using System.Threading.Tasks;

namespace BuildingBlocks.Core.Domain.DomainEvents
{
    public interface IDomainEventHandler<in T> where T : class, IDomainEvent
    {
        Task HandleAsync(T domainEvent);
    }
}