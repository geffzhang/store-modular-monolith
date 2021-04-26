using System;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Messaging.Transport;
using Common.Persistence.Mongo;
using Microsoft.Extensions.Logging;

namespace Common.Messaging.Commands
{
    [Decorator]
    internal sealed class UnitOfWorkCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
    {
        private readonly IExceptionToMessageMapperResolver _exceptionToMessageMapperResolver;
        private readonly ICommandHandler<T> _handler;
        private readonly ILogger<UnitOfWorkCommandHandlerDecorator<T>> _logger;
        private readonly ITransport _messageBroker;
        private readonly MongoOptions _options;
        private readonly IMongoSessionFactory _sessionFactory;

        public UnitOfWorkCommandHandlerDecorator(ICommandHandler<T> handler,
            IMongoSessionFactory sessionFactory, IExceptionToMessageMapperResolver exceptionToMessageMapperResolver,
            ITransport messageBroker, MongoOptions options, ILogger<UnitOfWorkCommandHandlerDecorator<T>> logger)
        {
            _handler = handler;
            _sessionFactory = sessionFactory;
            _exceptionToMessageMapperResolver = exceptionToMessageMapperResolver;
            _messageBroker = messageBroker;
            _options = options;
            _logger = logger;
        }

        public async Task HandleAsync(T command)
        {
            if (_options.DisableTransactions)
            {
                await TryHandleAsync(command);
                return;
            }

            using var session = await _sessionFactory.CreateAsync();
            await TryHandleAsync(command, () => session.CommitTransactionAsync(),
                () => session.AbortTransactionAsync());
        }

        private async Task TryHandleAsync(T command, Func<Task> onSuccess = null, Func<Task> onError = null)
        {
            try
            {
                await _handler.HandleAsync(command);
                if (onSuccess is { }) await onSuccess();
            }
            catch (Exception exception)
            {
                if (onError is { }) await onError();

                // Not a background processing
                if (command.Id == Guid.Empty) throw;

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