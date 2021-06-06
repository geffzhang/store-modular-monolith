using System;

namespace Common.Messaging.Outbox
{
    public class OutboxMessage 
    {
        /// <summary>
        /// Id of message.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// CorrelationId of message
        /// </summary>
        public Guid CorrelationId { get; set; }
        /// <summary>
        /// Name of message
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Full name of message type.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Message payload - serialized to JSON.
        /// </summary>
        public string Payload { get; set; }
        /// <summary>
        /// occuring datetime
        /// </summary>
        public DateTime OccurredOn { get; set; }
        /// <summary>
        /// processed datetime
        /// </summary>
        public DateTime? SentAt { get; set; }
    }
}

