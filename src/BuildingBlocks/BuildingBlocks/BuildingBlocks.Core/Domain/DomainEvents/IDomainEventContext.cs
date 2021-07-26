using System.Collections.Generic;

namespace BuildingBlocks.Core.Domain.DomainEvents
{
    public interface IDomainEventContext
    {
        IEnumerable<IDomainEvent> GetDomainEvents();
    }
}