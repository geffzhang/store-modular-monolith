using System.Threading.Channels;
using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Messaging.Transport.InMemory
{
    public interface IChannelFactory
    {
        ChannelReader<TMessage> GetReader<TMessage>() where TMessage : class, IMessage;
        ChannelWriter<TMessage> GetWriter<TMessage>() where TMessage : class, IMessage;
    }
}