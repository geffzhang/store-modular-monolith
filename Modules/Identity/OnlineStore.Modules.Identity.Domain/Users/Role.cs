using System.Collections.Generic;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users
{
    //https://ardalis.com/enum-alternatives-in-c/
    //https://ardalis.com/listing-strongly-typed-enum-options-in-c/
    public class Role : ValueObject
    {
        // public static Role User => new("User", "User", "User role type.");
        // public static Role Admin => new("Admin", "Admin", "Admin role type.");

        public static Role Customer =>
            new(SecurityConstants.Roles.Customer.Name, SecurityConstants.Roles.Customer.Code, SecurityConstants.Roles
                    .Customer.Description,
                Permission.CreateUsers,
                Permission.DeleteUsers,
                Permission.EditUsers,
                Permission.InviteUsers,
                Permission.ViewUsers,
                Permission.SeeUsersDetail);

        public static Role Admin =>
            new(SecurityConstants.Roles.Admin.Name, SecurityConstants.Roles.Customer.Code, SecurityConstants.Roles
                    .Customer.Description,
                Permission.EditAdmins,
                Permission.CreateAdmins,
                Permission.DeleteAdmins,
                Permission.ViewAdmins,
                Permission.SeeAdminsDetail);

        public string Name { get; }
        public string Code { get; }
        public string Description { get; }

        public IList<Permission> Permissions { get; }

        private Role(string name, string code, string description = null, params Permission[] permissions)
        {
            Name = name;
            Code = code;
            Permissions = permissions;
            Description = description;
        }

        public static Role Of(string name, string code, string description = null, params Permission[] permissions)
        {
            return new(name, code, description, permissions);
        }

        public static IEnumerable<Role> AllRoles()
        {
            return new[] {Customer, Admin,};
        }

        public static readonly IEnumerable<Role> B2BRoles = new List<Role>();

        public static bool IsValid(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;

            role = role.ToLowerInvariant();

            return role == Customer.Name.ToLower() || role == Admin.Name.ToLower();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}