// using System.Linq;
// using System.Threading.Tasks;
// using BuildingBlocks.Core;
// using BuildingBlocks.Core.Domain.DomainEvents;
// using BuildingBlocks.Core.Messaging.Serialization;
// using BuildingBlocks.Cqrs.Commands;
// using Microsoft.Extensions.Logging;
// using Microsoft.EntityFrameworkCore;
//
// namespace BuildingBlocks.Persistence.MSSQL.Decorators
// {
//     [Decorator]
//     internal class TransactionalCommandHandlerDecorator<T> : ICommandHandler<T>
//         where T : class, ICommand, ITransactionalCommand
//     {
//         private readonly ICommandHandler<T> _handler;
//         private readonly IDbFacadeResolver _dbFacadeResolver;
//         private readonly ICommandProcessor _commandProcessor;
//         private readonly IMessageSerializer _serializer;
//         private readonly IDomainEventContext _domainEventContext;
//         private readonly ILogger<TransactionalCommandHandlerDecorator<T>> _logger;
//
//         public TransactionalCommandHandlerDecorator(ICommandHandler<T> handler,
//             IDbFacadeResolver dbFacadeResolver,
//             ICommandProcessor commandProcessor,
//             IMessageSerializer serializer,
//             IDomainEventContext domainEventContext,
//             ILogger<TransactionalCommandHandlerDecorator<T>> logger)
//         {
//             _handler = handler;
//             _dbFacadeResolver = dbFacadeResolver;
//             _commandProcessor = commandProcessor;
//             _serializer = serializer;
//             _domainEventContext = domainEventContext;
//             _logger = logger;
//         }
//
//         public async Task HandleAsync(T command)
//         {
//             if (command is not { })
//             {
//                 await _handler.HandleAsync(command);
//             }
//
//             _logger.LogInformation("{Prefix} Handled command {Request}",
//                 nameof(TransactionalCommandHandlerDecorator<T>), typeof(T).FullName);
//             _logger.LogDebug("{Prefix} Handled command {MediatRRequest} with content {RequestContent}",
//                 nameof(TransactionalCommandHandlerDecorator<T>), typeof(T).FullName, _serializer.Serialize(command));
//             _logger.LogInformation("{Prefix} Open the transaction for {MediatRRequest}",
//                 nameof(TransactionalCommandHandlerDecorator<T>), typeof(T).FullName);
//             var strategy = _dbFacadeResolver.Database.CreateExecutionStrategy();
//
//             await strategy.ExecuteAsync(operation: async () =>
//             {
//                 await using var transaction = await _dbFacadeResolver.Database.BeginTransactionAsync();
//
//                 await _handler.HandleAsync(command);
//                 _logger.LogInformation("{Prefix} Executed the {MediatRRequest} request",
//                     nameof(TransactionalCommandHandlerDecorator<T>), typeof(T).FullName);
//
//                 await transaction.CommitAsync();
//
//                 var domainEvents = _domainEventContext.GetDomainEvents().ToList();
//                 _logger.LogInformation("{Prefix} Published domain events for {MediatRRequest}",
//                     nameof(TransactionalCommandHandlerDecorator<T>),
//                     typeof(TransactionalCommandHandlerDecorator<T>).FullName);
//
//                 var tasks = domainEvents
//                     .Select(async @event =>
//                     {
//                         await _commandProcessor.PublishDomainEventAsync(@event);
//                         _logger.LogDebug(
//                             "{Prefix} Published domain event {DomainEventName} with payload {DomainEventContent}",
//                             nameof(TransactionalCommandHandlerDecorator<T>), @event.GetType().FullName,
//                             _serializer.Serialize(@event));
//                     });
//
//                 await Task.WhenAll(tasks);
//             });
//         }
//     }
// }