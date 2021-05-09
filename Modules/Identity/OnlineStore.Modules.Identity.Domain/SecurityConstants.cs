using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Domain
{
    public static class SecurityConstants
    {
        public const string ANONYMOUS_USERNAME = "Anonymous";

        public static class Claims
        {
            public const string PERMISSION_CLAIM_TYPE = "permission";
            public const char PERMISSION_CLAIM_TYPE_DELIMITER = ';';
            public const string USER_NAME_CLAIM_TYPE = "username";
            public const string LIMITED_PERMISSIONS_CLAIM_TYPE = "limited_permissions";
        }

        public static class Roles
        {
            public static readonly Role.Role Customer =
                Role.Role.Of(nameof(Customer), nameof(Customer), "Customer Role.");

            public static readonly Role.Role Admin =
                Role.Role.Of(nameof(Admin), nameof(Admin), "Admins Role.");


            public static readonly IEnumerable<Role.Role> AllRoles = new List<Role.Role>() {Customer, Admin};

            public static readonly IEnumerable<Role.Role> B2BRoles = new List<Role.Role>() {Customer};
        }

        public static class SystemRoles
        {
            public const string CUSTOMER = "__customer";
            public const string MANAGER = "__manager";
            public const string ADMINISTRATOR = "__administrator";
        }

        public static class Permissions
        {
            public const string MODULE_QUERY = "module:read";
            public const string MODULE_ACCESS = "module:access";
            public const string MODULE_MANAGE = "module:manage";

            public const string SETTING_QUERY = "setting:read";
            public const string SETTING_ACCESS = "setting:access";
            public const string SETTING_UPDATE = "setting:update";

            // Users Permissions
            public const string CAN_SEE_USERS_DETAIL = "user:view";
            public const string CAN_EDIT_USERS = "user:edit";
            public const string CAN_INVITE_USERS = "user:invite";
            public const string CAN_CREATE_USERS = "user:create";
            public const string CAN_DELETE_USERS = "user:delete";
            public const string CAN_VIEW_USERS = "user:view";

            // Admin Permissions
            public const string CAN_SEE_ADMINS_DETAIL = "admin:view";
            public const string CAN_EDIT_ADMINS = "admin:edit";
            public const string CAN_CREATE_ADMINS = "admin:create";
            public const string CAN_DELETE_ADMINS = "admin:delete";
            public const string CAN_VIEW_ADMINS = "admin:view";

            // Security
            public const string SECURITY_QUERY = "security:read";
            public const string SECURITY_CREATE = "security:create";
            public const string SECURITY_ACCESS = "security:access";
            public const string SECURITY_UPDATE = "security:update";
            public const string SECURITY_DELETE = "security:delete";
            public const string SECURITY_VERIFY_EMAIL = "security:verifyEmail";
            public const string SECURITY_LOGIN_ON_BEHALF = "security:loginOnBehalf";

            public static string[] AllPermissions { get; } = new[]
            {
                CAN_SEE_USERS_DETAIL, CAN_EDIT_USERS, CAN_INVITE_USERS, CAN_CREATE_USERS, CAN_DELETE_USERS, CAN_VIEW_USERS,
                CAN_SEE_ADMINS_DETAIL, CAN_EDIT_ADMINS, CAN_CREATE_ADMINS, CAN_DELETE_ADMINS, CAN_VIEW_ADMINS, MODULE_QUERY,
                MODULE_ACCESS, MODULE_MANAGE, SETTING_QUERY, SETTING_ACCESS, SETTING_UPDATE, SECURITY_QUERY,
                SECURITY_CREATE, SECURITY_ACCESS, SECURITY_UPDATE, SECURITY_DELETE, SECURITY_VERIFY_EMAIL,
                SECURITY_LOGIN_ON_BEHALF
            };
        }

        public static class Changes
        {
            public const string USER_UPDATED = "UserUpdated";
            public const string USER_PASSWORD_CHANGED = "UserPasswordChanged";
            public const string ROLE_ADDED = "RoleAdded";
            public const string ROLE_REMOVED = "RoleRemoved";
        }
    }
}