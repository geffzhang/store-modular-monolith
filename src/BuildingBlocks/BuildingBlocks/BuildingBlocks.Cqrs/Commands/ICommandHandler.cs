namespace BuildingBlocks.Cqrs.Commands
{
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Unit> where TCommand : IRequest<Unit>
    {
    }

    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : IRequest<TResult>
    {
    }
}