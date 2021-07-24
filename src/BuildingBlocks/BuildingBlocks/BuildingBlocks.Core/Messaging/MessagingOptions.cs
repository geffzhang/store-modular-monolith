using BuildingBlocks.Core.Messaging.Inbox;
using BuildingBlocks.Core.Messaging.Outbox;

namespace BuildingBlocks.Core.Messaging
{
    public class MessagingOptions
    {
        public InboxOptions Inbox { get; set; }
        public OutboxOptions Outbox { get; set; }
    }
}