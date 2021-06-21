using System;

namespace Common.Domain.Types
{
    public interface IEntity<TId, out TIdentity> where TIdentity : IdentityBase<TId>
    {
        TIdentity Id { get; }
    }

    public interface IEntity : IEntity<Guid, IdentityBase<Guid>>
    {
    }
}