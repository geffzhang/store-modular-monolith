using System;

namespace Common.Messaging.Inbox
{
    public class InboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CorrelationId { get; set; }
        public DateTime OccurredOn { get; set; }
        public string Name { get; set; }
        public string Module { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public DateTime? ProcessedDate { get; set; }
    }
}