using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles.Mappings;

namespace Common.Tests.Integration.Constants
{
    public class UsersConstants
    {
        public static class AdminUserMock
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
            public static readonly IList<ApplicationRole> Roles = new List<ApplicationRole>()
            {
                Role.Admin.ToApplicationRole()
            };
        }
        public static class NormalUserMock
        {
            public const string UserId = "5073f0f0-855a-48e6-9168-d4e20f1d2840";
            public const string Name = "customer";
            public const string FirstName = "customer";
            public const string LastName = "customer";
            public const string UserName = "customer";
            public const string UserType = "Customer";
            public const bool IsAdministrator = false;
            public const bool IsActive = true;
            public const string UserEmail = "customer@customer.com";
            public const string Password = "123";
            public static readonly IList<ApplicationRole> Roles = new List<ApplicationRole>()
            {
                Role.Customer.ToApplicationRole()
            };
        }
    }
}     