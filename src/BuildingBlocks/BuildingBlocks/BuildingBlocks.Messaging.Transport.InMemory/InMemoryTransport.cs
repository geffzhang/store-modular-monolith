using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Messaging.Transport;

namespace BuildingBlocks.Messaging.Transport.InMemory
{
    public class InMemoryTransport : ITransport
    {
        private readonly IPublisher _publisher;
        private readonly ISubscriber _subscriber;

        public InMemoryTransport(IPublisher publisher, ISubscriber subscriber)
        {
            _publisher = publisher;
            _subscriber = subscriber;
        }

        public async Task PublishAsync<T>(T message) where T : class, IMessage
        {
            await _publisher.PublishAsync(message);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _subscriber.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _subscriber.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}