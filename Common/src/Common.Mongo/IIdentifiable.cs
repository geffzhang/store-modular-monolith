namespace Common.Mongo
{
    public interface IIdentifiable<out T>
    {
        T Id { get; }
    }
}