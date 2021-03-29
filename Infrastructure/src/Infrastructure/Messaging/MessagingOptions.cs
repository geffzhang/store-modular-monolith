using Infrastructure.Messaging.Inbox;
using Infrastructure.Messaging.Outbox;

namespace Infrastructure.Messaging
{
    internal class MessagingOptions
    {
        public bool UseBackgroundDispatcher { get; set; }
        public InboxOptions Inbox { get; set; }
        public OutboxOptions Outbox { get; set; }
    }
}