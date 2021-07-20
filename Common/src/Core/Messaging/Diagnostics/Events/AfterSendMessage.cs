namespace Common.Core.Messaging.Diagnostics.Events
{
    public class AfterSendMessage
    {
        public AfterSendMessage(IMessage eventData)
            => EventData = eventData;
        public IMessage EventData { get; }
    }
}