using System.Threading.Channels;
using Common.Messaging.Events;

namespace Common.Messaging.Transport.InMemory
{
    internal class MessageChannel : IMessageChannel
    {
        private readonly Channel<IMessage> _messages = Channel.CreateUnbounded<IMessage>();
        public ChannelReader<IMessage> Reader => _messages.Reader;
        public ChannelWriter<IMessage> Writer => _messages.Writer;
    }
}