using System;

namespace Common.Messaging.Events
{
    public abstract class IntegrationEventBase : IIntegrationEvent
    {
        protected IntegrationEventBase(Guid id)
        {
            Id = id;
            OccurredOn = DateTime.Now;
        }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime OccurredOn { get; }
    }
}