using System.Threading.Channels;
using Common.Messaging.Events;

namespace Common.Messaging.Transport.InMemory
{
    internal class MessageChannel : IMessageChannel
    {
        private readonly Channel<IIntegrationEvent> _messages = Channel.CreateUnbounded<IIntegrationEvent>();
        public ChannelReader<IIntegrationEvent> Reader => _messages.Reader;
        public ChannelWriter<IIntegrationEvent> Writer => _messages.Writer;
    }
}