using System.Threading.Tasks;
using Common.Messaging.Commands;
using Common.Messaging.Events;
using Common.Messaging.Queries;

namespace Common.Dispatcher
{
    internal class InProcessDispatcher: IInProcessDispatcher
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public InProcessDispatcher(ICommandDispatcher commandDispatcher, IEventDispatcher eventDispatcher,
            IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _eventDispatcher = eventDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        public Task SendAsync<T>(T command) where T : class, ICommand
            => _commandDispatcher.SendAsync(command);

        public Task PublishAsync<T>(T @event) where T : class, IEvent
            => _eventDispatcher.PublishAsync(@event);

        public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
            => _queryDispatcher.QueryAsync(query);
    }
}