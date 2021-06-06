using System.Threading.Channels;
using Common.Messaging.Events;

namespace Common.Messaging.Transport.InMemory
{
    internal interface IMessageChannel
    {
        ChannelReader<IMessage> Reader { get; }
        ChannelWriter<IMessage> Writer { get; }
    }
}