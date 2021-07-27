using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Domain.DomainEvents;
using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Cqrs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Persistence.MSSQL
{
    public class TransactionalCqrsRequestMiddleware<TRequest, TResponse> : IRequestMiddleware<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly IDomainEventContext _domainEventContext;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IDbFacadeResolver _dbFacadeResolver;
        private readonly IMessageSerializer _serializer;
        private readonly ILogger<TransactionalCqrsRequestMiddleware<TRequest, TResponse>> _logger;

        public TransactionalCqrsRequestMiddleware(IDbFacadeResolver dbFacadeResolver,
            IDomainEventContext domainEventContext,
            IDomainEventDispatcher domainEventDispatcher,
            IMessageSerializer serializer,
            ILogger<TransactionalCqrsRequestMiddleware<TRequest, TResponse>> logger)
        {
            _domainEventContext = domainEventContext ?? throw new ArgumentNullException(nameof(domainEventContext));
            _domainEventDispatcher = domainEventDispatcher;
            _dbFacadeResolver = dbFacadeResolver ?? throw new ArgumentNullException(nameof(dbFacadeResolver));
            _serializer = serializer;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken,
            HandleRequestDelegate<TRequest, TResponse> next)
        {
            if (request is not ITransactionalRequest)
            {
                return await next(request, cancellationToken);
            }

            _logger.LogInformation("{Prefix} Handled command {MediatRRequest}",
                nameof(TransactionalCqrsRequestMiddleware<TRequest, TResponse>),
                typeof(TRequest).FullName);
            _logger.LogDebug("{Prefix} Handled command {MediatRRequest} with content {RequestContent}",
                nameof(TransactionalCqrsRequestMiddleware<TRequest, TResponse>), typeof(TRequest).FullName,
                _serializer.Serialize(request));
            _logger.LogInformation("{Prefix} Open the transaction for {MediatRRequest}",
                nameof(TransactionalCqrsRequestMiddleware<TRequest, TResponse>), typeof(TRequest).FullName);
            var strategy = _dbFacadeResolver.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                var isInnerTransaction = _dbFacadeResolver.Database.CurrentTransaction is not null;
                var transaction = _dbFacadeResolver.Database.CurrentTransaction ??
                                  await _dbFacadeResolver.Database.BeginTransactionAsync(cancellationToken);

                var response = await next(request, cancellationToken);

                var domainEvents = _domainEventContext.GetDomainEvents().ToList();
                _logger.LogInformation("{Prefix} Published domain events for {MediatRRequest}",
                    nameof(TransactionalCqrsRequestMiddleware<TRequest, TResponse>), typeof(TRequest).FullName);

                var tasks = domainEvents
                    .Select(async @event =>
                    {
                        await _domainEventDispatcher.DispatchAsync(@event);
                        _logger.LogDebug(
                            "{Prefix} Published domain event {DomainEventName} with payload {DomainEventContent}",
                            nameof(TransactionalCqrsRequestMiddleware<TRequest, TResponse>), @event.GetType().FullName,
                            _serializer.Serialize(@event));
                    });

                _logger.LogInformation("{Prefix} Executed the {MediatRRequest} request",
                    nameof(TransactionalCqrsRequestMiddleware<TRequest, TResponse>), typeof(TRequest).FullName);

                await Task.WhenAll(tasks);

                if (isInnerTransaction == false)
                    await transaction.CommitAsync(cancellationToken);

                return response;
            });
        }
    }
}