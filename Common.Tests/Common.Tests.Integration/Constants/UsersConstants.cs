using System.Collections.Generic;

namespace Common.Tests.Integration.Constants
{
    public class UsersConstants
    {
        public static class AdminUser
        {
            public const string UserId = "4073f0f0-855a-48e6-9168-d4e20f1d2839";
            public const string Name = "admin";
            public const string FirstName = "admin";
            public const string LastName = "admin";
            public const string UserName = "admin";
            public const string UserType = "Administrator";
            public const bool IsAdministrator = true;
            public const bool IsActive = true;
            public const string UserEmail = "admin@admin.com";
            public const string Password = "admin";
            public static IList<string> Roles => new List<string>
            {
                "admin"
            };
        }
    }
}     