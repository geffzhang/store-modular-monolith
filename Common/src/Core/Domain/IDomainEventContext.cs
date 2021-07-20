using System.Collections.Generic;

namespace Common.Core.Domain
{
    public interface IDomainEventContext
    {
        IEnumerable<IDomainEvent> GetDomainEvents();
    }
}