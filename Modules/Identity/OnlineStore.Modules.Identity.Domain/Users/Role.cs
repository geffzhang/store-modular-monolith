using System.Collections.Generic;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users
{
    //https://ardalis.com/enum-alternatives-in-c/
    //https://ardalis.com/listing-strongly-typed-enum-options-in-c/
    public class Role : ValueObject
    {
        public static Role User => new("User");
        public static Role Admin => new("Admin");
        public static Role Organization => SecurityConstants.Roles.Organization;
        public static Role Vendor => new("Vendor");
        public static Role Employee => new("Employee");
        public static Role PurchasingAgent => SecurityConstants.Roles.PurchasingAgent;
        public static Role StoreAdministrator => SecurityConstants.Roles.StoreAdministrator;
        public static Role StoreManager => SecurityConstants.Roles.StoreManager;
        public static Role OrganizationEmployee => SecurityConstants.Roles.OrganizationEmployee;

        public string Name { get; }
        public IList<UserPermission> Permissions { get; }

        private Role(string name, params Permission[] permissions)
        {
            Name = name;
            Permissions = permissions;
        }

        public static Role Of(string name, params Permission[] permissions)
        {
            return new(name, permissions);
        }

        public static IEnumerable<Role> AllRoles()
        {
            return new[]
            {
                User, Admin, Organization, OrganizationEmployee, Vendor, Employee, PurchasingAgent,
                StoreAdministrator, StoreManager
            };
        }

        public static readonly IEnumerable<Role> B2BRoles = new[]
        {
            Organization, OrganizationEmployee, Vendor, PurchasingAgent
        };

        public static bool IsValid(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;

            role = role.ToLowerInvariant();

            return role == User.Name.ToLower() || role == Admin.Name.ToLower() || role == Organization.Name.ToLower()
                   || role == Vendor.Name.ToLower() || role == Employee.Name.ToLower();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Permissions;
        }
    }
}