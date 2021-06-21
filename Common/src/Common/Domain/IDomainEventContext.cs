using System.Collections.Generic;

namespace Common.Domain
{
    public interface IDomainEventContext
    {
        IEnumerable<IDomainEvent> GetDomainEvents();
    }
}