using System;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Messaging.Transport;
using Common.Persistence.Mongo;
using Microsoft.Extensions.Logging;

namespace Common.Messaging.Events
{
    [Decorator]
    internal sealed class UnitOfWorkEventHandlerDecorator<T> : IIntegrationEventHandler<T>
        where T : class, IIntegrationEvent
    {
        private readonly IExceptionToMessageMapperResolver _exceptionToMessageMapperResolver;
        private readonly IIntegrationEventHandler<T> _handler;
        private readonly ILogger<UnitOfWorkEventHandlerDecorator<T>> _logger;
        private readonly ITransport _messageBroker;
        private readonly MongoOptions _options;
        private readonly IMongoSessionFactory _sessionFactory;

        public UnitOfWorkEventHandlerDecorator(IIntegrationEventHandler<T> handler, IMongoSessionFactory sessionFactory,
            IExceptionToMessageMapperResolver exceptionToMessageMapperResolver, ITransport messageBroker,
            MongoOptions options, ILogger<UnitOfWorkEventHandlerDecorator<T>> logger)
        {
            _handler = handler;
            _sessionFactory = sessionFactory;
            _exceptionToMessageMapperResolver = exceptionToMessageMapperResolver;
            _messageBroker = messageBroker;
            _options = options;
            _logger = logger;
        }

        public async Task HandleAsync(T @event)
        {
            if (_options.DisableTransactions)
            {
                await TryHandleAsync(@event);
                return;
            }

            using var session = await _sessionFactory.CreateAsync();
            await TryHandleAsync(@event, () => session.CommitTransactionAsync(), () => session.AbortTransactionAsync());
        }

        private async Task TryHandleAsync(T @event, Func<Task> onSuccess = null, Func<Task> onError = null)
        {
            try
            {
                await _handler.HandleAsync(@event);
                if (onSuccess is { }) await onSuccess();
            }
            catch (Exception exception)
            {
                if (onError is { }) await onError();

                if (@event is IActionRejected) throw;

                var rejectedEvent = _exceptionToMessageMapperResolver.Map(exception);
                if (rejectedEvent is { })
                {
                    _logger.LogInformation("Publishing the rejected event...");
                    await _messageBroker.PublishAsync(rejectedEvent);
                }

                throw;
            }
        }
    }
}