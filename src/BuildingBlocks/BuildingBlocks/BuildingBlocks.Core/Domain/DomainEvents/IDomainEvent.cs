using System;

namespace BuildingBlocks.Core.Domain.DomainEvents
{
    public interface IDomainEvent 
    {
        public DateTime OccurredOn { get; }
        public Guid Id { get; set; }
    }
}