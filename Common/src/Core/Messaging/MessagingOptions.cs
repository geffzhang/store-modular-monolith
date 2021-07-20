using Common.Core.Messaging.Inbox;
using Common.Core.Messaging.Outbox;

namespace Common.Core.Messaging
{
    public class MessagingOptions
    {
        public bool UseBackgroundDispatcher { get; set; }
        public InboxOptions Inbox { get; set; }
        public OutboxOptions Outbox { get; set; }
    }
}