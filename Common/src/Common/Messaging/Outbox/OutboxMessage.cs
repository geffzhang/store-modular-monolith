using System;

namespace Common.Messaging.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
        public long ReceivedAt { get; set; }
        public long? SentAt { get; set; }
    }
}