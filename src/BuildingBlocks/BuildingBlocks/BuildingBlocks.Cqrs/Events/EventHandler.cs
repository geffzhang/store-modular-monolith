using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Cqrs.Events
{
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IRequest<Unit>
    {
        public async Task<Unit> HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
        {
            await HandleEventAsync(@event, cancellationToken);
            return Unit.Result;
        }

        protected abstract Task HandleEventAsync(TEvent @event, CancellationToken cancellationToken = default);
    }

    public abstract class EventHandler<TEvent, TResult> : IEventHandler<TEvent, TResult>
        where TEvent : IRequest<TResult>
    {
        public async Task<TResult> HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
        {
            return await HandleEventAsync(@event, cancellationToken);
        }

        protected abstract Task<TResult> HandleEventAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}