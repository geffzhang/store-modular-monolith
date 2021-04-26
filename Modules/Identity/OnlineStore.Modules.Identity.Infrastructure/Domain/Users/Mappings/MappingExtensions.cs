using System;
using System.Linq;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Mappings
{
    public static class MappingExtensions
    {
        public static ApplicationUser ToApplicationUser(this User user)
        {
            var permissions = user.Permissions.Select(x => x.Name).ToArray();

            var applicationUser = new ApplicationUser
            {
                Name = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Id = user.Id.Id.ToString(),
                Permissions = permissions,
                Roles = user.Roles.Select(x => x.ToApplicationRole()).ToList(),
                CreatedDate = user.CreatedDate,
                CreatedBy = user.CreatedBy,
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive,
                IsAdministrator = user.IsAdministrator,
                MemberId = user.MemberId,
                PasswordHash = user.Password,
                PhotoUrl = user.PhotoUrl,
                UserName = user.UserName,
                UserType = user.UserType,
                StoreId = user.StoreId,
            };

            return applicationUser;
        }

        public static User ToUser(this ApplicationUser appUser)
        {
            var permissions = appUser.Permissions.Select(x => Permission.Of(x, "User")).ToArray();

            return User.Of(new UserId(Guid.Parse(appUser.Id)), appUser.Email, appUser.FirstName, appUser.LastName,
                appUser.Name, appUser.UserName, appUser.Password, appUser.CreatedDate, permissions, appUser.UserType,
                appUser.Roles.Select(x => x.Name).ToList());
        }

        public static ApplicationRole ToApplicationRole(this Role role)
        {
            return new()
            {
                Description = role.Description, Id = role.Name, Name = role.Name, Permissions = role.Permissions
            };
        }

        public static Role ToRole(this ApplicationRole role)
        {
            return Role.Of(role.Name, role.Description, role.Permissions.ToArray());
        }
    }
}