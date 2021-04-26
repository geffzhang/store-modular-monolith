using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Domain
{
    public class Constants
    {
        public const string AnonymousUsername = "Anonymous";

        public static class Claims
        {
            public const string PermissionClaimType = "permission";
            public const string OperatorUserNameClaimType = "operatorname";
            public const string OperatorUserIdClaimType = "operatornameidentifier";
            public const string CurrencyClaimType = "currency";
        }

        public static class Roles
        {
            public const string Customer = "Customer";
            public const string Operator = "Operator";
            public const string Administrator = "Administrator";

            public static readonly Role Organization =
                Role.Of("Organization maintainer", Permissions.CanEditOrganization,
                    Permissions.CanViewUsers, Permissions.CanCreateUsers, Permissions.CanSeeOrganizationDetail,
                    Permissions.CanDeleteUsers, Permissions.CanEditOrganization, Permissions.CanEditUsers,
                    Permissions.CanInviteUsers);

            public static readonly Role OrganizationEmployee = Role.Of("Organization employee",
                Permissions.CanSeeOrganizationDetail, Permissions.CanViewUsers);

            public static readonly Role PurchasingAgent = Role.Of("Purchasing agent",
                Permissions.CanSeeOrganizationDetail, Permissions.CanViewUsers
            );

            public static readonly Role StoreAdministrator = Role.Of("Store administrator",
                Permissions.CanViewUsers, Permissions.CanCreateUsers, Permissions.CanSeeOrganizationDetail,
                Permissions.CanDeleteUsers, Permissions.CanEditOrganization, Permissions.CanEditUsers,
                Permissions.CanInviteUsers, Permissions.CanViewOrders, Permissions.CanChangeOrderStatus);

            public static readonly Role StoreManager = Role.Of("Store manager",
                Permissions.CanSeeOrganizationDetail, Permissions.CanViewOrders,
                Permissions.CanChangeOrderStatus);

            public static readonly IEnumerable<Role> AllRoles = new[]
            {
                Organization, OrganizationEmployee, PurchasingAgent, StoreAdministrator, StoreManager
            };

            public static readonly IEnumerable<Role> B2BRoles = new[]
            {
                Organization, OrganizationEmployee, PurchasingAgent
            };
        }

        public static class Permissions
        {
            public const string CanResetCache = "cache:reset";
            public const string CanSeeOrganizationDetail = "storefront:organization:view";
            public const string CanEditOrganization = "storefront:organization:edit";
            public const string CanInviteUsers = "storefront:user:invite";
            public const string CanCreateUsers = "storefront:user:create";
            public const string CanEditUsers = "storefront:user:edit";
            public const string CanDeleteUsers = "storefront:user:delete";
            public const string CanViewUsers = "storefront:user:view";
            public const string CanViewOrders = "storefront:order:view";
            public const string CanChangeOrderStatus = "storefront:order:changestatus";

            public static readonly IEnumerable<string> AllPermissions = new[]
            {
                CanViewUsers, CanResetCache, CanSeeOrganizationDetail, CanEditOrganization, CanInviteUsers,
                CanEditUsers, CanDeleteUsers, CanCreateUsers, CanViewOrders, CanChangeOrderStatus
            };
        }
    }
}