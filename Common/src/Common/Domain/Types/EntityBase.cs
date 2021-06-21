using System;
using System.Collections.Generic;

namespace Common.Domain.Types
{
    public abstract class EntityBase<TId, TIdentity> : IEntity<TId, TIdentity>, IEquatable<EntityBase<TId, TIdentity>>
        where TIdentity : IdentityBase<TId>
    {
        public TIdentity Id { get; protected set; }

        protected EntityBase()
        {
        }
        protected EntityBase(TIdentity id)
        {
            Id = id;
        }

        public DateTime Created { get; protected set; } = DateTime.Now;
        public DateTime? Updated { get; protected set; }

        public bool Equals(EntityBase<TId, TIdentity> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TIdentity>.Default.Equals(Id, other.Id) && Created.Equals(other.Created) &&
                   Nullable.Equals(Updated, other.Updated);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntityBase<TId, TIdentity>) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Created, Updated);
        }

        public override string ToString()
        {
            return $"{GetType().Name}#[Identity={Id}]";
        }

        public static bool operator ==(EntityBase<TId, TIdentity> a, EntityBase<TId, TIdentity> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(EntityBase<TId, TIdentity> a, EntityBase<TId, TIdentity> b)
        {
            return !(a == b);
        }
    }

    public abstract class EntityBase : EntityBase<Guid, IdentityBase<Guid>>
    {
        protected EntityBase(IdentityBase<Guid> id) : base(id)
        {
        }
    }
}