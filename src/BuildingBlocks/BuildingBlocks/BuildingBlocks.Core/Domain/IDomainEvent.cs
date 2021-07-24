using System;

namespace BuildingBlocks.Core.Domain
{
    public interface IDomainEvent 
    {
        public DateTime OccurredOn { get; }
        public Guid Id { get; set; }
    }
}