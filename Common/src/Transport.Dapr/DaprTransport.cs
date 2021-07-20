using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core.Messaging;
using Common.Core.Messaging.Transport;
using Common.Messaging;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Transport.Dapr
{
    public class DaprTransport : ITransport
    {
        private readonly DaprClient _daprClient;
        private readonly IOptions<DaprEventBusOptions> _options;
        private readonly ILogger<DaprTransport> _logger;

        public DaprTransport(DaprClient daprClient, IOptions<DaprEventBusOptions> options,
            ILogger<DaprTransport> logger)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync<T>(T message) where T : class, IMessage
        {
            var attr = (DaprPubSubNameAttribute) Attribute.GetCustomAttribute(typeof(IMessage),
                typeof(DaprPubSubNameAttribute));

            var pubsubName = _options.Value.PubSubName ?? "pubsub";

            if (attr is not null)
            {
                pubsubName = attr.PubSubName;
            }

            var topicName = message.GetType().Name;
            _logger.LogInformation("Publishing event {@Event} to {PubsubName}.{TopicName}", message, pubsubName,
                topicName);
            await _daprClient.PublishEventAsync(pubsubName, topicName, message);
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}