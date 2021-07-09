using System.Threading.Channels;
using Common.Messaging.Events;

namespace Common.Messaging.Transport.InMemory
{
    internal interface IMessageChannel
    {
        ChannelReader<IIntegrationEvent> Reader { get; }
        ChannelWriter<IIntegrationEvent> Writer { get; }
    }
}