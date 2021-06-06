using System;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Messaging.Commands;
using Common.Messaging.Transport;
using Common.Persistence;
using Microsoft.Extensions.Logging;

namespace Common.Domain.Dispatching
{
    [Decorator]
    internal sealed class DomainEventsDispatcherCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
    {
        private readonly IExceptionToMessageMapperResolver _exceptionToMessageMapperResolver;
        private readonly ICommandHandler<T> _decorated;
        private readonly ILogger<DomainEventsDispatcherCommandHandlerDecorator<T>> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransport _messageBroker;

        public DomainEventsDispatcherCommandHandlerDecorator(ICommandHandler<T> decorated, IExceptionToMessageMapperResolver exceptionToMessageMapperResolver,
            ITransport messageBroker,  ILogger<DomainEventsDispatcherCommandHandlerDecorator<T>> logger,
            IUnitOfWork unitOfWork)
        {
            _decorated = decorated;
            _exceptionToMessageMapperResolver = exceptionToMessageMapperResolver;
            _messageBroker = messageBroker;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(T command)
        {
            try
            {
                await _decorated.HandleAsync(command);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception exception)
            {
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