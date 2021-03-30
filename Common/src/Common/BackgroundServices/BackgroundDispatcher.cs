using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Messaging;
using Common.Modules;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.BackgroundServices
{
    internal sealed class BackgroundDispatcher : BackgroundService
    {
        private readonly IMessageChannel _channel;
        private readonly IModuleClient _moduleClient;
        private readonly ILogger<BackgroundDispatcher> _logger;

        public BackgroundDispatcher(IMessageChannel channel, IModuleClient moduleClient,
            ILogger<BackgroundDispatcher> logger)
        {
            _channel = channel;
            _moduleClient = moduleClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Running the background event dispatcher...");
            await foreach (var @event in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await _moduleClient.SendAsync(@event);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                }
            }

            _logger.LogInformation("Finished running the background event dispatcher.");
        }
    }
}