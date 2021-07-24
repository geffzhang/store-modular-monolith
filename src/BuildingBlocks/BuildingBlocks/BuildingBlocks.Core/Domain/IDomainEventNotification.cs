using System;

namespace BuildingBlocks.Core.Domain
{
    public interface IDomainEventNotification<TEventType> : IDomainEventNotification
    {
        TEventType DomainEvent { get; set; }
    }

    public interface IDomainEventNotification 
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}