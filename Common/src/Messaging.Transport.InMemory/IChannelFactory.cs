using System.Threading.Channels;
using Common.Core.Messaging;

namespace Messaging.Transport.InMemory
{
    public interface IChannelFactory
    {
        ChannelReader<TMessage> GetReader<TMessage>() where TMessage : class, IMessage;
        ChannelWriter<TMessage> GetWriter<TMessage>() where TMessage : class, IMessage;
    }
}