using System;

namespace BuildingBlocks.Core.Domain.DomainEvents
{
    public abstract class DomainEventBase : IDomainEvent
    {
        protected DomainEventBase()
        {
            OccurredOn = DateTime.Now;
            Id = Guid.NewGuid();
        }

        public int Version { get; set; }
        public DateTime OccurredOn { get; }
        public Guid Id { get; set; }
    }
}