namespace Common.Identity
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
    }
}