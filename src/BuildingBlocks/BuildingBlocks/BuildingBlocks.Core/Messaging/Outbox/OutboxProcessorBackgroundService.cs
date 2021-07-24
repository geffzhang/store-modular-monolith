using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Core.Messaging.Outbox
{
    internal class OutboxProcessorBackgroundService : BackgroundService
    {
        private readonly bool _enabled;
        private readonly TimeSpan _interval;
        private readonly ILogger<OutboxProcessorBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OutboxProcessorBackgroundService(IServiceScopeFactory serviceScopeFactory, IOptions<OutboxOptions> outboxOptions,
            ILogger<OutboxProcessorBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _interval = outboxOptions.Value?.Interval ?? TimeSpan.FromSeconds(1);
            _enabled = outboxOptions.Value?.Enabled ?? false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_enabled)
            {
                _logger.LogInformation("Outbox is disabled");
                return;
            }

            _logger.LogInformation("Outbox is enabled");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("Started processing outbox messages...");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                        await outbox.PublishUnsentAsync();
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError("There was an error when processing outbox.");
                        _logger.LogError(exception, exception.Message);
                    }
                }

                stopwatch.Stop();
                _logger.LogTrace($"Finished processing outbox messages in {stopwatch.ElapsedMilliseconds} ms.");
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}