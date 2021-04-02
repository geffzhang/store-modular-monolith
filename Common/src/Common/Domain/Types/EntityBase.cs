using System;

namespace Common.Domain.Types
{
    public abstract class EntityBase<TId, TIdentity>
        where TIdentity : IdentityBase<TId>
    {
        protected TIdentity Id;

        protected EntityBase(TIdentity id)
        {
            Id = id;
        }

        protected EntityBase()
        {
        }

        public DateTime Created { get; protected set; } = DateTime.Now;
        public DateTime? Updated { get; protected set; }
    }
}