using System;
using System.Threading.Tasks;
using Common.Messaging.Events;
using Common.Messaging.Transport;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Messaging.Dapr
{
    public class DaprTransport : ITransport
    {
        private readonly DaprClient _daprClient;
        private readonly IOptions<DaprEventBusOptions> _options;
        private readonly ILogger<DaprTransport> _logger;

        public DaprTransport(DaprClient daprClient, IOptions<DaprEventBusOptions> options, ILogger<DaprTransport> logger)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync(params IIntegrationEvent[] messages)
        {
            var attr = (DaprPubSubNameAttribute) Attribute.GetCustomAttribute(typeof(IMessage),
                typeof(DaprPubSubNameAttribute));

            var pubsubName = _options.Value.PubSubName ?? "pubsub";

            if (attr is not null)
            {
                pubsubName = attr.PubSubName;
            }

            foreach (var message in messages)
            {
                var topicName = messages.GetType().Name;
                _logger.LogInformation("Publishing event {@Event} to {PubsubName}.{TopicName}", message, pubsubName, topicName);
                await _daprClient.PublishEventAsync(pubsubName, topicName, message);
            }
        }
    }
}