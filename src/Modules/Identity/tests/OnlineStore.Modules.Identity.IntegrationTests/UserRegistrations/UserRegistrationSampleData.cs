using System;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Types;

namespace OnlineStore.Modules.Identity.IntegrationTests.UserRegistrations
{
    public struct UserRegistrationSampleData
    {
        public static Guid Id => Guid.NewGuid();
        public static string Email => "test@mail.com";
        public static string FirstName => "test";
        public static string LastName => "test";
        public static string Name => "test";
        public static string UserName => "test";
        public static string PhoneNumber => "test";
        public static string Password => "admin123456";
        public static bool EmailConfirmed => false;
        public static bool IsAdministrator => true;
        public static string PhotoUrl => null;
        public static UserType UserType => UserType.Administrator;
        public static string Status { get; }
        public static bool LockoutEnabled => true;
        public static bool IsActive => true;
        public static IEnumerable<string> Roles => new List<string> {Role.Admin.Name};
        public static IEnumerable<string> Permissions => Role.Admin.Permissions.Select(x => x.Name);
    }
}