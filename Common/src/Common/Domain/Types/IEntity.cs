namespace Common.Domain.Types
{
    public interface IEntity<TIdentity>
    {
        TIdentity Id { get;  }
    }
}