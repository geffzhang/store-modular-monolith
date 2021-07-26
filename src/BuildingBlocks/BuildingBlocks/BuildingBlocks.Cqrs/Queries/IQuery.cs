namespace BuildingBlocks.Cqrs.Queries
{
    //Marker
    public interface IQuery
    {
    }

    public interface IQuery<T> : IRequest<T>, IQuery
    {
    }
}