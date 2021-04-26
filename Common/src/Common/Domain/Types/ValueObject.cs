using System;
using System.Collections.Generic;
using System.Linq;
using Common.Domain.Exceptions;

namespace Common.Domain.Types
{
    public abstract class ValueObject : IEquatable<ValueObject>,  ICloneable
    {
        public bool Equals(ValueObject obj)
        {
            return Equals(obj as object);
        }

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;

            if (ReferenceEquals(null, obj)) return false;

            if (GetType() != obj.GetType()) return false;

            var valueObject = (ValueObject) obj;

            return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents().Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return current * 23 + (obj?.GetHashCode() ?? 0);
                }
            });
        }

        public static bool operator ==(ValueObject object1, ValueObject object2)
        {
            if (ReferenceEquals(object1, null) && ReferenceEquals(object2, null))
                return true;

            if (ReferenceEquals(object1, null) || ReferenceEquals(object2, null))
                return false;

            return object1.Equals(object2);
        }

        public static bool operator !=(ValueObject object1, ValueObject object2)
        {
            return !(object1 == object2);
        }

        protected static void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken()) throw new BusinessRuleValidationException(rule);
        }
        
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}