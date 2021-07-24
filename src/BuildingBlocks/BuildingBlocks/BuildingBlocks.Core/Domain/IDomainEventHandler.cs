using System.Threading.Tasks;

namespace BuildingBlocks.Core.Domain
{
    public interface IDomainEventHandler<in T> where T : class, IDomainEvent
    {
        Task HandleAsync(T domainEvent);
    }
}