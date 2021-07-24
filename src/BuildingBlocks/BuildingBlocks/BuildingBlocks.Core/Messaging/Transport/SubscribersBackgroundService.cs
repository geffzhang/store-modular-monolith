using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Messaging.Transport
{
    public class SubscribersBackgroundService : BackgroundService
    {
        private readonly IEnumerable<ISubscriber> _subscribers;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubscribersBackgroundService> _logger;

        public SubscribersBackgroundService(IEnumerable<ISubscriber> subscribers, IServiceProvider serviceProvider,
            ILogger<SubscribersBackgroundService> logger)
        {
            _subscribers = subscribers ?? throw new ArgumentNullException(nameof(subscribers));
            _serviceProvider = serviceProvider;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_subscribers.Select(s => s.StopAsync(cancellationToken)));

            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = _subscribers.Select(s => s.StartAsync(stoppingToken));
            await Task.WhenAll(tasks);
        }
    }
}