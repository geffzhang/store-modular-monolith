namespace BuildingBlocks.Persistence.Mongo
{
    public interface IIdentifiable<out T>
    {
        T Id { get; }
    }
}