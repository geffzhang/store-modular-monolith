using System.Threading.Tasks;
using Common.Messaging.Events;

namespace Common.Messaging.Transport.InMemory
{
    internal class InMemoryAsyncMessageDispatcher : IAsyncMessageDispatcher
    {
        private readonly IMessageChannel _channel;

        public InMemoryAsyncMessageDispatcher(IMessageChannel channel)
        {
            _channel = channel;
        }

        public async Task PublishAsync<T>(T message) where T : class, IMessage
        {
            await _channel.Writer.WriteAsync(message);
        }
    }
}