namespace BuildingBlocks.Cqrs.Events
{
    public interface IEventHandler<in TEvent> : IRequestHandler<TEvent, Unit> where TEvent : IRequest<Unit>
    {
    }

    public interface IEventHandler<in TEvent, TResult> : IRequestHandler<TEvent, TResult>
        where TEvent : IRequest<TResult>
    {
    }
}