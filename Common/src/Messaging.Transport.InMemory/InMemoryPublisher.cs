using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Common.Core.Messaging;
using Common.Core.Messaging.Transport;
using Common.Web.Contexts;
using Messaging.Transport.InMemory.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Messaging.Transport.InMemory
{
    public class InMemoryPublisher : IPublisher
    {
        private readonly ILogger<InMemoryPublisher> _logger;
        private readonly IChannelFactory _channelFactory;
        private readonly InMemoryProducerDiagnostics _producerDiagnostics;
        private readonly IServiceProvider _serviceProvider;

        public InMemoryPublisher(ILogger<InMemoryPublisher> logger, IChannelFactory channelFactory,
            InMemoryProducerDiagnostics producerDiagnostics, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _channelFactory = channelFactory;
            _producerDiagnostics = producerDiagnostics;
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<T>(T message) where T : class, IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message.CorrelationId == Guid.Empty)
            {
                var context = _serviceProvider.GetRequiredService<ICorrelationContextAccessor>();
                message.CorrelationId = Guid.Parse(context.CorrelationContext.CorrelationId);
            }

            if (message.Id == Guid.Empty)
            {
                message.Id = Guid.NewGuid();
            }

            if (_channelFactory.GetWriter<T>() is not null)
            {
                _logger.LogInformation("publishing message '{message.Id}'...", message.Id);
                //ProducerDiagnostics
                _producerDiagnostics.StartActivity(message);
                await _channelFactory.GetWriter<T>().WriteAsync(message).ConfigureAwait(false);
                _producerDiagnostics.StopActivity(message);
            }
            else
            {
                _logger.LogWarning(
                    "no suitable publisher found for message '{message.Id}' with type '{typeof(T).FullName}' !",
                    message.Id, typeof(T).FullName);
            }
        }
    }
}