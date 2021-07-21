using Common.Core.Messaging.Inbox;
using Common.Core.Messaging.Outbox;

namespace Common.Core.Messaging
{
    public class MessagingOptions
    {
        public InboxOptions Inbox { get; set; }
        public OutboxOptions Outbox { get; set; }
    }
}