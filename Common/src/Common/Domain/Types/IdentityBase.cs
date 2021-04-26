using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Domain.Types
{
    /// <summary>
    ///     Avoiding Primitive Obsession
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class IdentityBase<TId> : ValueObject
    {
        protected IdentityBase(TId id)
        {
            Id = id;
        }

        public TId Id { get; protected set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
        }
        
        public static implicit operator TId(IdentityBase<TId> identity)
            => identity.Id;

        public override string ToString()
        {
            return $"{GetType().Name}:{Id}";
        }
    }

    public static class IdentityFactory
    {
        public static TIdentity Create<TIdentity, TId>() where TIdentity : IdentityBase<TId>
        {
            return Create<TIdentity, TId>(default);
        }

        public static TIdentity Create<TIdentity, TId>(TId id) where TIdentity : IdentityBase<TId>
        {
            if (id == null) return null;

            var identityConstructor = typeof(TIdentity)
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(x => x.GetParameters().Length > 0);

            var instance = identityConstructor?.Invoke(new object[] {id});

            return (TIdentity) instance;
        }
    }
}