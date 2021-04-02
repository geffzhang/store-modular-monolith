using System.Threading.Channels;

namespace Common.Messaging.Transport.InMemory
{
    internal interface IMessageChannel
    {
        ChannelReader<IMessage> Reader { get; }
        ChannelWriter<IMessage> Writer { get; }
    }
}