using System;
using Common.Messaging.Events;

namespace Common.Domain
{
    public interface IDomainEvent 
    {
        public DateTime OccurredOn { get; }
        public Guid Id { get; }
    }
    
}