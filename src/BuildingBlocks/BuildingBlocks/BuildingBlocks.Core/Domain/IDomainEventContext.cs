using System.Collections.Generic;

namespace BuildingBlocks.Core.Domain
{
    public interface IDomainEventContext
    {
        IEnumerable<IDomainEvent> GetDomainEvents();
    }
}