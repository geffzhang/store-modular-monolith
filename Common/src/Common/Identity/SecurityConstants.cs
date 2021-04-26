using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Domain
{
    public class SecurityConstants
    {
        public const string AnonymousUsername = "Anonymous";

        public static class Claims
        {
            public const string PermissionClaimType = "permission";
            public const string OperatorUserNameClaimType = "operatorname";
            public const string OperatorUserIdClaimType = "operatornameidentifier";
            public const string CurrencyClaimType = "currency";
        }

        // public static class Roles
        // {
        //     public const string Customer = "Customer";
        //     public const string Operator = "Operator";
        //     public const string Administrator = "Administrator";
        //
        //     public static readonly Role Organization =
        //         Role.Of("Organization maintainer", Permissions.CanEditOrganization,
        //             Permissions.CanViewUsers, Permissions.CanCreateUsers, Permissions.CanSeeOrganizationDetail,
        //             Permissions.CanDeleteUsers, Permissions.CanEditOrganization, Permissions.CanEditUsers,
        //             Permissions.CanInviteUsers);
        //
        //     public static readonly Role OrganizationEmployee = Role.Of("Organization employee",
        //         Permissions.CanSeeOrganizationDetail, Permissions.CanViewUsers);
        //
        //     public static readonly Role PurchasingAgent = Role.Of("Purchasing agent",
        //         Permissions.CanSeeOrganizationDetail, Permissions.CanViewUsers
        //     );
        //
        //     public static readonly Role StoreAdministrator = Role.Of("Store administrator",
        //         Permissions.CanViewUsers, Permissions.CanCreateUsers, Permissions.CanSeeOrganizationDetail,
        //         Permissions.CanDeleteUsers, Permissions.CanEditOrganization, Permissions.CanEditUsers,
        //         Permissions.CanInviteUsers, Permissions.CanViewOrders, Permissions.CanChangeOrderStatus);
        //
        //     public static readonly Role StoreManager = Role.Of("Store manager",
        //         Permissions.CanSeeOrganizationDetail, Permissions.CanViewOrders,
        //         Permissions.CanChangeOrderStatus);
        //
        //     public static readonly IEnumerable<Role> AllRoles = new[]
        //     {
        //         Organization, OrganizationEmployee, PurchasingAgent, StoreAdministrator, StoreManager
        //     };
        //
        //     public static readonly IEnumerable<Role> B2BRoles = new[]
        //     {
        //         Organization, OrganizationEmployee, PurchasingAgent
        //     };
        // }

        public static class SystemRoles
        {
            public const string Customer = "__customer";
            public const string Manager = "__manager";
            public const string Administrator = "__administrator";
        }

        public static class Permissions
        {
            public const string ResetCache = "cache:reset";

            public const string AssetAccess = "platform:asset:access",
                AssetDelete = "platform:asset:delete",
                AssetUpdate = "platform:asset:update",
                AssetCreate = "platform:asset:create",
                AssetRead = "platform:asset:read";

            public const string ModuleQuery = "platform:module:read",
                ModuleAccess = "platform:module:access",
                ModuleManage = "platform:module:manage";

            public const string SettingQuery = "platform:setting:read",
                SettingAccess = "platform:setting:access",
                SettingUpdate = "platform:setting:update";

            public const string DynamicPropertiesQuery = "platform:dynamic_properties:read",
                DynamicPropertiesCreate = "platform:dynamic_properties:create",
                DynamicPropertiesAccess = "platform:dynamic_properties:access",
                DynamicPropertiesUpdate = "platform:dynamic_properties:update",
                DynamicPropertiesDelete = "platform:dynamic_properties:delete";

            public const string SecurityQuery = "platform:security:read",
                SecurityCreate = "platform:security:create",
                SecurityAccess = "platform:security:access",
                SecurityUpdate = "platform:security:update",
                SecurityDelete = "platform:security:delete",
                SecurityVerifyEmail = "platform:security:verifyEmail",
                SecurityLoginOnBehalf = "platform:security:loginOnBehalf";

            public const string BackgroundJobsManage = "background_jobs:manage";

            public const string PlatformExportImportAccess = "platform:exportImport:access",
                PlatformImport = "platform:import",
                PlatformExport = "platform:export";

            public static string[] AllPermissions { get; } = new[]
            {
                ResetCache, AssetAccess, AssetDelete, AssetUpdate, AssetCreate, AssetRead, ModuleQuery,
                ModuleAccess, ModuleManage, SettingQuery, SettingAccess, SettingUpdate, DynamicPropertiesQuery,
                DynamicPropertiesCreate, DynamicPropertiesAccess, DynamicPropertiesUpdate, DynamicPropertiesDelete,
                SecurityQuery, SecurityCreate, SecurityAccess, SecurityUpdate, SecurityDelete, BackgroundJobsManage,
                PlatformExportImportAccess, PlatformImport, PlatformExport, SecurityLoginOnBehalf,
                SecurityVerifyEmail
            };
        }

        public static class Changes
        {
            public const string UserUpdated = "UserUpdated";
            public const string UserPasswordChanged = "UserPasswordChanged";
            public const string RoleAdded = "RoleAdded";
            public const string RoleRemoved = "RoleRemoved";
        }
    }
}