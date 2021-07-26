// using System.Threading.Tasks;
// using BuildingBlocks.Core.Extensions;
// using BuildingBlocks.Cqrs.Commands;
//
// namespace BuildingBlocks.Core.Messaging.Inbox
// {
//     [Decorator]
//     internal class InboxCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
//     {
//         private readonly ICommandHandler<T> _handler;
//         private readonly IInbox _inbox;
//         private readonly string _module;
//
//         public InboxCommandHandlerDecorator(ICommandHandler<T> handler, IInbox inbox)
//         {
//             _handler = handler;
//             _inbox = inbox;
//             _module = _handler.GetModuleName();
//         }
//
//         public Task HandleAsync(T command)
//         {
//             var message = new InboxMessage();
//             return _inbox.Enabled
//                 ? _inbox.HandleAsync(message, () => _handler.HandleAsync(command), _module)
//                 : _handler.HandleAsync(command);
//         }
//     }
// }