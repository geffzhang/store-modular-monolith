namespace Infrastructure.Messaging.Queries
{
    //Marker
    public interface IQuery
    {
    }

    public interface IQuery<T> : IQuery
    {
    }
}