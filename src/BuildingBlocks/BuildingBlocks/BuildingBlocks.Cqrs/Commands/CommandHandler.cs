﻿using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Cqrs.Commands
{
    public abstract class CommandHandler<TEvent> : ICommandHandler<TEvent> where TEvent : IRequest<Unit>
    {
        public async Task<Unit> HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
        {
            await HandleEventAsync(@event, cancellationToken);
            return Unit.Result;
        }

        protected abstract Task HandleEventAsync(TEvent @event,CancellationToken cancellationToken = default);
    }

    public abstract class CommandHandler<TEvent, TResult> : ICommandHandler<TEvent, TResult>
        where TEvent : IRequest<TResult>
    {
        public async Task<TResult> HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
        {
            return await HandleEventAsync(@event, cancellationToken);
        }

        protected abstract Task<TResult> HandleEventAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}