using System.Collections.Generic;
using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Permissions;

namespace OnlineStore.Modules.Identity.Domain.Role
{
    //https://ardalis.com/enum-alternatives-in-c/
    //https://ardalis.com/listing-strongly-typed-enum-options-in-c/
    public class Role : ValueObject
    {
        public static Role User => new("User", "User role type.");
        public static Role Admin => new("Admin", "Admin role type.");

        public string Name { get; }
        public string Description { get; }

        public IList<Permission> Permissions { get; }

        private Role(string name, string description = null, params Permission[] permissions)
        {
            Name = name;
            Permissions = permissions;
            Description = description;
        }

        public static Role Of(string name, string description = null, params Permission[] permissions)
        {
            return new(name, description, permissions);
        }

        public static IEnumerable<Role> AllRoles()
        {
            return new[] {User, Admin,};
        }

        public static readonly IEnumerable<Role> B2BRoles = new List<Role>();

        public static bool IsValid(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;

            role = role.ToLowerInvariant();

            return role == User.Name.ToLower() || role == Admin.Name.ToLower();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}