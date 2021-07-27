using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Logging.Serilog
{
    public class MessagingLoggingMiddleware<TMessage> : IMessageMiddleware<TMessage>
        where TMessage : IMessage
    {
        private readonly ILogger<MessagingLoggingMiddleware<TMessage>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessagingLoggingMiddleware(ILogger<MessagingLoggingMiddleware<TMessage>> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RunAsync(TMessage message, IMessageContext messageContext,
            CancellationToken cancellationToken, HandleMessageDelegate<TMessage> next)
        {
            const string prefix = nameof(MessagingLoggingMiddleware<TMessage>);
            _logger.LogInformation("[{Prefix}] Handle message={X-MessageData}", prefix, typeof(TMessage).Name);

            var timer = new Stopwatch();
            timer.Start();

            await next(message, messageContext, cancellationToken);

            timer.Stop();
            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
            {
                _logger.LogWarning("[{Perf-Possible}] The Message {X-MessageData} took {TimeTaken} seconds.",
                    prefix, typeof(TMessage).Name, timeTaken.Seconds);
            }

            _logger.LogInformation("[{Prefix}] Handled {X-MessageData}", prefix, typeof(TMessage).Name);
        }
    }
}