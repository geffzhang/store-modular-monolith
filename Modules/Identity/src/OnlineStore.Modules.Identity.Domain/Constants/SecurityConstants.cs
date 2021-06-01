using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Domain.Constants
{
    public static class SecurityConstants
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
                Role.Of(nameof(Customer), nameof(Customer), "Customer Role.");

            public static readonly Role Admin =
                Role.Of(nameof(Admin), nameof(Admin), "Admins Role.");


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
            public const string SeeUsersDetail = "user:view";
            public const string EditUsers = "user:edit";
            public const string InviteUsers = "user:invite";
            public const string CreateUsers = "user:create";
            public const string DeleteUsers = "user:delete";
            public const string ViewUsers = "user:view";

            // Admin Permissions
            public const string SeeAdminsDetail = "admin:view";
            public const string EditAdmins = "admin:edit";
            public const string CreateAdmins = "admin:create";
            public const string DeleteAdmins = "admin:delete";
            public const string ViewAdmins = "admin:view";

            // Security
            public const string SecurityQuery = "security:read";
            public const string SecurityCreate = "security:create";
            public const string SecurityAccess = "security:access";
            public const string SecurityUpdate = "security:update";
            public const string SecurityDelete = "security:delete";
            public const string SecurityVerifyEmail = "security:verifyEmail";
            public const string SecurityLoginOnBehalf = "security:loginOnBehalf";

            public static string[] AllPermissions { get; } = new[]
            {
                SeeUsersDetail, EditUsers, InviteUsers, CreateUsers, DeleteUsers, ViewUsers,
                SeeAdminsDetail, EditAdmins, CreateAdmins, DeleteAdmins, ViewAdmins, ModuleQuery,
                ModuleAccess, ModuleManage, SettingQuery, SettingAccess, SettingUpdate, SecurityQuery,
                SecurityCreate, SecurityAccess, SecurityUpdate, SecurityDelete, SecurityVerifyEmail,
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