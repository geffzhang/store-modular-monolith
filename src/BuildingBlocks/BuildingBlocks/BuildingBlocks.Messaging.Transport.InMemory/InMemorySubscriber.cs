using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Messaging.Transport;
using BuildingBlocks.Messaging.Transport.InMemory.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Transport.InMemory
{
    public class InMemorySubscriber<TMessage> : ISubscriber where TMessage : class, IMessage
    {
        private readonly IMessageProcessor _messageProcessor;
        private readonly InMemoryConsumerDiagnostics _consumerDiagnostics;
        private readonly IServiceProvider _serviceProvider;
        private readonly IChannelFactory _channelFactory;
        private readonly ILogger<InMemorySubscriber<TMessage>> _logger;

        public InMemorySubscriber(IMessageProcessor messageProcessor,
            InMemoryConsumerDiagnostics consumerDiagnostics,
            IServiceProvider serviceProvider,
            IChannelFactory channelFactory,
            ILogger<InMemorySubscriber<TMessage>> logger)
        {
            _messageProcessor = messageProcessor;
            _consumerDiagnostics = consumerDiagnostics;
            _serviceProvider = serviceProvider;
            _channelFactory = channelFactory;
            _channelFactory = channelFactory;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_channelFactory.GetReader<TMessage>() == null)
                return Task.CompletedTask;

            return Task.Factory.StartNew(async () => await ListenToMessages(cancellationToken),
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Current);
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            //https://dotnetcoretutorials.com/2020/11/24/using-channels-in-net-core-part-2-advanced-channels/
            _channelFactory.GetWriter<TMessage>().Complete();
            return Task.CompletedTask;
        }

        private async Task ListenToMessages(CancellationToken cancellationToken)
        {
            var reader = _channelFactory.GetReader<TMessage>();

            if (reader == null)
                return;

            //https://dotnetcoretutorials.com/2020/11/24/using-channels-in-net-core-part-1-getting-started/
            await foreach (var message in reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    //ConsumerDiagnostics
                    _consumerDiagnostics.StartActivity(message);
                    await _messageProcessor.ProcessAsync(message, null, cancellationToken);
                    _consumerDiagnostics.StopActivity(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        $"an exception has occurred while processing '{message.GetType().FullName}' message '{message.Id}': {e.Message}");
                }
            }
        }
    }
}