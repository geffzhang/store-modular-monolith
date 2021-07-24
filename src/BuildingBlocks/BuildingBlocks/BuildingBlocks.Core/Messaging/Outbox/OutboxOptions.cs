using System;

namespace BuildingBlocks.Core.Messaging.Outbox
{
    public class OutboxOptions
    {
        public bool Enabled { get; set; }
        public string CollectionName { get; set; } = "outbox";
        public TimeSpan? Interval { get; set; }
        public bool UseBackgroundDispatcher { get; set; } = true;
    }
}