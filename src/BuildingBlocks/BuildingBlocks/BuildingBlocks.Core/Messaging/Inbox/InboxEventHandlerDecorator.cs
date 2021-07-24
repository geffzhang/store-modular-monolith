namespace BuildingBlocks.Core.Messaging.Inbox
{
    // [Decorator]
    // internal class InboxEventHandlerDecorator<T> : IMessageHandler<T> where T : class, IIntegrationEvent
    // {
    //     private readonly IMessageHandler<T> _handler;
    //     private readonly IInbox _inbox;
    //     private readonly string _module;
    //
    //     public InboxEventHandlerDecorator(IMessageHandler<T> handler, IInbox inbox)
    //     {
    //         _handler = handler;
    //         _inbox = inbox;
    //         _module = _handler.GetModuleName();
    //     }
    //
    //     public Task HandleAsync(T @event)
    //     {
    //         var message = new InboxMessage();
    //         return _inbox.Enabled
    //             ? _inbox.HandleAsync(message, () => _handler.HandleAsync(@event), _module)
    //             : _handler.HandleAsync(@event);
    //     }
    // }
}