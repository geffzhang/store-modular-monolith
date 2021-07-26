// using System;
// using System.Threading.Tasks;
// using BuildingBlocks.Core.Exceptions;
// using BuildingBlocks.Core.Messaging.Transport;
// using BuildingBlocks.Cqrs.Commands;
// using BuildingBlocks.Persistence.MSSQL;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
//
// namespace BuildingBlocks.Core.Domain.DomainEvents
// {
//     [Decorator]
//     internal sealed class DomainEventsDispatcherCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
//     {
//         private readonly IExceptionToMessageMapperResolver _exceptionToMessageMapperResolver;
//         private readonly ICommandHandler<T> _decorated;
//         private readonly ILogger<DomainEventsDispatcherCommandHandlerDecorator<T>> _logger;
//         private readonly IDomainEventDispatcher _domainEventDispatcher;
//         private readonly ISqlDbContext _sqlDbContext;
//         private readonly ITransport _messageBroker;
//
//         public DomainEventsDispatcherCommandHandlerDecorator(ICommandHandler<T> decorated,
//             IExceptionToMessageMapperResolver exceptionToMessageMapperResolver,
//             ITransport messageBroker,
//             ILogger<DomainEventsDispatcherCommandHandlerDecorator<T>> logger,
//             IDomainEventDispatcher domainEventDispatcher,
//             ISqlDbContext sqlDbContext)
//         {
//             _decorated = decorated;
//             _exceptionToMessageMapperResolver = exceptionToMessageMapperResolver;
//             _messageBroker = messageBroker;
//             _logger = logger;
//             _domainEventDispatcher = domainEventDispatcher;
//             _sqlDbContext = sqlDbContext;
//         }
//
//         public async Task HandleAsync(T command)
//         {
//             try
//             {
//                 var strategy = _sqlDbContext.Database.CreateExecutionStrategy();
//                 await strategy.ExecuteAsync(operation: async () =>
//                 {
//                     // Achieving atomicity
//                     await _sqlDbContext.BeginTransactionAsync();
//
//                     await _decorated.HandleAsync(command);
//                     await _domainEventDispatcher.DispatchAsync();
//
//                     await _sqlDbContext.CommitTransactionAsync();
//                 });
//             }
//             catch (Exception exception)
//             {
//                 var rejectedEvent = _exceptionToMessageMapperResolver.Map(exception);
//                 if (rejectedEvent is { })
//                 {
//                     _logger.LogInformation("Publishing the rejected event...");
//                     await _messageBroker.PublishAsync(rejectedEvent);
//                 }
//
//                 throw;
//             }
//         }
//     }
// }