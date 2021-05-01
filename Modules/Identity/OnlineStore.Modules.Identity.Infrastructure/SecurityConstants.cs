namespace OnlineStore.Modules.Identity.Infrastructure
{
    public partial class SecurityConstants
    {
        public const string AnonymousUsername = "Anonymous";

        public static class Claims
        {
            public const string PermissionClaimType = "permission";
            public const string OperatorUserNameClaimType = "operatorname";
            public const string OperatorUserIdClaimType = "operatornameidentifier";
            public const string CurrencyClaimType = "currency";
        }

        public static class SystemRoles
        {
            public const string Customer = "__customer";
            public const string Manager = "__manager";
            public const string Administrator = "__administrator";
        }

        public static class Changes
        {
            public const string UserUpdated = "UserUpdated";
            public const string UserPasswordChanged = "UserPasswordChanged";
            public const string RoleAdded = "RoleAdded";
            public const string RoleRemoved = "RoleRemoved";
        }

        public static class Permissions
        {
            public const string ResetCache = "cache:reset";

            public const string AssetAccess = "asset:access",
                AssetDelete = "asset:delete",
                AssetUpdate = "asset:update",
                AssetCreate = "asset:create",
                AssetRead = "asset:read";

            public const string ModuleQuery = "module:read",
                ModuleAccess = "module:access",
                ModuleManage = "module:manage";

            public const string SettingQuery = "platformsetting:read",
                SettingAccess = "setting:access",
                SettingUpdate = "setting:update";


            public const string SecurityQuery = "security:read",
                SecurityCreate = "security:create",
                SecurityAccess = "security:access",
                SecurityUpdate = "security:update",
                SecurityDelete = "security:delete",
                SecurityVerifyEmail = "security:verifyEmail",
                SecurityLoginOnBehalf = "security:loginOnBehalf";

            public const string SecurityCallApi = "security:call_api";


            public static string[] AllPermissions { get; } =
            {
                ResetCache, AssetAccess, AssetDelete, AssetUpdate, AssetCreate, AssetRead, ModuleQuery,
                ModuleAccess, ModuleManage, SettingQuery, SettingAccess, SettingUpdate, SecurityQuery,
                SecurityCreate, SecurityAccess, SecurityUpdate, SecurityDelete, SecurityCallApi,
            };
        }
    }
}