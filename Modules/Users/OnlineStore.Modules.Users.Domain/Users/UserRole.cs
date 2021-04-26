using System.Collections.Generic;
using Common.Domain.Types;

namespace OnlineStore.Modules.Users.Domain.Users
{
    //https://enterprisecraftsmanship.com/posts/value-object-better-implementation/
    //https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/
    public class UserRole : ValueObject
    {
        private UserRole(string value)
        {
            Value = value;
        }
        
        public static UserRole User => new(nameof(User));
        public static UserRole Admin => new(nameof(Admin));

        public string Value { get; }
        
        public static UserRole Of(string roleCode)
        {
            return new(roleCode);
        }

        public static bool IsValid(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;

            role = role.ToLowerInvariant();

            return role == User.Value.ToLower() || role == Admin.Value.ToLower();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        
        
        public static implicit operator string(UserRole userRole) => userRole.Value;

        public static implicit operator UserRole(string role) => new UserRole(role);
    }
}