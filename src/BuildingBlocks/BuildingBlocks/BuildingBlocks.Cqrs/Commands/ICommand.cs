using System;

namespace BuildingBlocks.Cqrs.Commands
{
    public interface ICommand<TResult>: IRequest<TResult>
    {
        public Guid Id { get; }
    }

    public interface ICommand: IRequest<Unit>
    {
        public Guid Id { get; }
    }
}