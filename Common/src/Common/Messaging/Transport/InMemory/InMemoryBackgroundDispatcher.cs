using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Messaging.Events;
using Common.Modules;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.Messaging.Transport.InMemory
{
    internal sealed class InMemoryBackgroundDispatcher : BackgroundService
    {
        private readonly IMessageChannel _channel;
        private readonly ILogger<InMemoryBackgroundDispatcher> _logger;
        private readonly IIntegrationEventDispatcher _integrationEventDispatcher;

        public InMemoryBackgroundDispatcher(IMessageChannel channel,
            IIntegrationEventDispatcher integrationEventDispatcher,
            ILogger<InMemoryBackgroundDispatcher> logger)
        {
            _channel = channel;
            _integrationEventDispatcher = integrationEventDispatcher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Running the background event dispatcher...");
            await foreach (var @event in _channel.Reader.ReadAllAsync(stoppingToken))
                try
                {
                    await _integrationEventDispatcher.PublishAsync(@event);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                }

            _logger.LogInformation("Finished running the background event dispatcher.");
        }
    }
}