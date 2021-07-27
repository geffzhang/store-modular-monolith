using System;

namespace BuildingBlocks.Cqrs.Commands
{
    public interface ICommand<TResult> : IRequest<TResult>, ITransactionalRequest
    {
        public Guid Id { get; }
    }

    public interface ICommand : ICommand<Unit>
    {
    }
}