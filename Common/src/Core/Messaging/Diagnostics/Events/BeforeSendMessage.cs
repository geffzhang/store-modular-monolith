namespace Common.Core.Messaging.Diagnostics.Events
{
    public class BeforeSendMessage
    {
        public BeforeSendMessage(IMessage eventData)
            => EventData = eventData;
        public IMessage EventData { get; }
    }
}