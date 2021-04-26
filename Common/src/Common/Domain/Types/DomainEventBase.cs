using System;

namespace Common.Domain.Types
{
    public abstract class DomainEventBase : IDomainEvent
    {
        protected DomainEventBase()
        {
            OccurredOn = DateTime.Now;
        }
        public int Version { get; set; }
        public DateTime OccurredOn { get; }
    }
}