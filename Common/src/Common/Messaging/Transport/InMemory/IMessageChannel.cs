using System.Threading.Channels;

namespace Common.Messaging
{
    internal interface IMessageChannel
    {
        ChannelReader<IMessage> Reader { get; }
        ChannelWriter<IMessage> Writer { get; }
    }
}