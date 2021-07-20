using System;

namespace Common.Core.Domain
{
    public interface IDomainEvent 
    {
        public DateTime OccurredOn { get; }
        public Guid Id { get; set; }
    }
}