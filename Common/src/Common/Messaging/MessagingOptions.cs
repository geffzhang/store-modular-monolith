using Common.Messaging.Inbox;
using Common.Messaging.Outbox;

namespace Common.Messaging
{
    public class MessagingOptions
    {
        public bool UseBackgroundDispatcher { get; set; }
        public InboxOptions Inbox { get; set; }
        public OutboxOptions Outbox { get; set; }
    }
}