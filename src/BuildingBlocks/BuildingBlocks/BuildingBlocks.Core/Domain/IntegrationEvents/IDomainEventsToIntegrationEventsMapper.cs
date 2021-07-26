using System.Collections.Generic;
using BuildingBlocks.Core.Domain.DomainEvents;

namespace BuildingBlocks.Core.Domain.IntegrationEvents
{
    public interface IDomainEventsToIntegrationEventsMapper
    {
        public IEnumerable<dynamic?> Map(params IDomainEvent[] events);
    }
}