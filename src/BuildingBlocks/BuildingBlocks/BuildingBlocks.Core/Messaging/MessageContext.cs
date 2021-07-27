namespace BuildingBlocks.Core.Messaging
{
    public class MessageContext : IMessageContext
    {
        public static MessageContext Default => new MessageContext();
    }
}