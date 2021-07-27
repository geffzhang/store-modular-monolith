namespace BuildingBlocks.Cqrs.Events
{
    public interface IEvent<TResult> : IRequest<TResult>, ITransactionalRequest
    {
    }

    public interface IEvent : IEvent<Unit>
    {
    }
}