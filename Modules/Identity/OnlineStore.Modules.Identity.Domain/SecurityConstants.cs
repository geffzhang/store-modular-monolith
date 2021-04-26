using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Domain
{
    public class SecurityConstants
    {
        public const string AnonymousUsername = "Anonymous";

        public static class Claims
        {
            public const string PermissionClaimType = "permission";
            public const char PermissionClaimTypeDelimiter = ';';
            public const string UserNameClaimType = "username";
            public const string LimitedPermissionsClaimType = "limited_permissions";
        }

        public static class Roles
        {
            public static readonly Role Customer =
                Role.Of(nameof(Customer), "Customer Role.",
                    Permission.Of(Permissions.CanCreateUsers, nameof(Customer)),
                    Permission.Of(Permissions.CanDeleteUsers, nameof(Customer)),
                    Permission.Of(Permissions.CanEditUsers, nameof(Customer)),
                    Permission.Of(Permissions.CanInviteUsers, nameof(Customer)),
                    Permission.Of(Permissions.CanViewUsers, nameof(Customer)),
                    Permission.Of(Permissions.CanSeeUsersDetail, nameof(Customer)));

            public static readonly Role Admin =
                Role.Of(nameof(Admin), "Admins Role.", Permission.Of(Permissions.CanEditAdmins, nameof(Admin)),
                    Permission.Of(Permissions.CanCreateAdmins, nameof(Admin)),
                    Permission.Of(Permissions.CanDeleteAdmins, nameof(Admin)),
                    Permission.Of(Permissions.CanViewAdmins, nameof(Admin)),
                    Permission.Of(Permissions.CanSeeAdminsDetail, nameof(Admin)));


            public static readonly IEnumerable<Role> AllRoles = new List<Role>() {Customer, Admin};

            public static readonly IEnumerable<Role> B2BRoles = new List<Role>() {Customer};
        }

        public static class SystemRoles
        {
            public const string Customer = "__customer";
            public const string Manager = "__manager";
            public const string Administrator = "__administrator";
        }

        public static class Permissions
        {
            public const string ModuleQuery = "module:read";
            public const string ModuleAccess = "module:access";
            public const string ModuleManage = "module:manage";

            public const string SettingQuery = "setting:read";
            public const string SettingAccess = "setting:access";
            public const string SettingUpdate = "setting:update";

            // Users Permissions
            public const string CanSeeUsersDetail = "user:view";
            public const string CanEditUsers = "user:edit";
            public const string CanInviteUsers = "user:invite";
            public const string CanCreateUsers = "user:create";
            public const string CanDeleteUsers = "user:delete";
            public const string CanViewUsers = "user:view";

            // Admin Permissions
            public const string CanSeeAdminsDetail = "admin:view";
            public const string CanEditAdmins = "admin:edit";
            public const string CanCreateAdmins = "admin:create";
            public const string CanDeleteAdmins = "admin:delete";
            public const string CanViewAdmins = "admin:view";

            //
            public const string SecurityQuery = "security:read";
            public const string SecurityCreate = "security:create";
            public const string SecurityAccess = "security:access";
            public const string SecurityUpdate = "security:update";
            public const string SecurityDelete = "security:delete";
            public const string SecurityVerifyEmail = "security:verifyEmail";
            public const string SecurityLoginOnBehalf = "security:loginOnBehalf";

            public static string[] AllPermissions { get; } = new[]
            {
                CanSeeUsersDetail, CanEditUsers, CanInviteUsers, CanCreateUsers, CanDeleteUsers,
                CanViewUsers, CanSeeAdminsDetail, CanEditAdmins, CanCreateAdmins, CanDeleteAdmins,
                CanViewAdmins, ModuleQuery, ModuleAccess, ModuleManage, SettingQuery, SettingAccess, SettingUpdate,
                SecurityQuery, SecurityCreate, SecurityAccess, SecurityUpdate, SecurityDelete, SecurityVerifyEmail,
                SecurityLoginOnBehalf
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