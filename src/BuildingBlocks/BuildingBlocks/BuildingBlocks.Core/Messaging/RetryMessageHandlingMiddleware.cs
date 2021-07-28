using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace BuildingBlocks.Core.Messaging
{
    public class RetryMessageHandlingMiddleware<TMessage> : IMessageMiddleware<TMessage> where TMessage : IMessage
    {
        private readonly IAsyncPolicy _retryPolicy;

        public RetryMessageHandlingMiddleware()
        {
            _retryPolicy = Policy.Handle<ArgumentOutOfRangeException>()
                .WaitAndRetryAsync(3,
                    i => TimeSpan.FromSeconds(i));
        }

        public Task RunAsync(TMessage message, IMessageContext messageContext, CancellationToken cancellationToken,
            HandleMessageDelegate<TMessage> next)
        {
            return _retryPolicy.ExecuteAsync(() => next(message, messageContext, cancellationToken));
        }
    }
}