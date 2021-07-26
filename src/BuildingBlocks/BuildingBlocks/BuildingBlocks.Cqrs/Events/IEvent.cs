namespace BuildingBlocks.Cqrs.Events
{
    public interface IEvent<TResult> : IRequest<TResult>
    {
    }

    public interface IEvent : IEvent<Unit>
    {
    }
}