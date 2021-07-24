using System.Diagnostics;
using System.Threading.Tasks;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Logging.Serilog
{
    [Decorator]
    internal sealed class MessageHandlerLoggingDecorator<T> : IMessageHandler<T> where T : class, IMessage
    {
        private readonly IMessageHandler<T> _handler;
        private readonly ILogger<IMessageHandler<T>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessageHandlerLoggingDecorator(IMessageHandler<T> handler, ILogger<IMessageHandler<T>> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _handler = handler;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(T message)
        {
            const string prefix = nameof(MessageHandlerLoggingDecorator<T>);
            _logger.LogInformation("[{Prefix}] Handle request={X-RequestData}", prefix, typeof(T).Name);

            var timer = new Stopwatch();
            timer.Start();

            await _handler.HandleAsync(message);

            timer.Stop();
            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
            {
                _logger.LogWarning("[{Perf-Possible}] The request {X-RequestData} took {TimeTaken} seconds.",
                    prefix, typeof(T).Name, timeTaken.Seconds);
            }

            _logger.LogInformation("[{Prefix}] Handled {X-RequestData}", prefix, typeof(T).Name);
        }
    }
}