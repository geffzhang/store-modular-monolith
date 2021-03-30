using System;

namespace Common.Messaging.Outbox
{
    internal class OutboxOptions
    {
        public bool Enabled { get; set; }
        public string CollectionName { get; set; }
        public TimeSpan? Interval { get; set; }
    }
}