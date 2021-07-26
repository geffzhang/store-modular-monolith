using System;
using System.Linq;
using BuildingBlocks.Core.Extensions;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users
{
    public static class MappingExtensions
    {
        public static ApplicationUser ToApplicationUser(this User user)
        {
            if (user is null)
                return null;

            var applicationUser = new ApplicationUser
            {
                Name = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Id = user.Id.Id.ToString(),
                Permissions = user.Permissions.ToList(),
                Roles = user.Roles.Select(x => x.ToApplicationRole()).ToList(),
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive,
                IsAdministrator = user.IsAdministrator,
                PhotoUrl = user.PhotoUrl,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType.ToString(),
                CreatedBy = user.CreatedBy,
                CreatedDate = user.CreatedDate,
                ModifiedBy = user.ModifiedBy,
                ModifiedDate = user.ModifiedDate
            };

            return applicationUser;
        }

        public static User ToUser(this ApplicationUser appUser)
        {
            if (appUser is null)
                return null;

            var userType = EnumUtility.SafeParse(appUser.UserType, UserType.Customer);

            var permissions = appUser.Permissions?.Select(x => Permission.Of(x.Name, "")).ToArray();
            var roles = appUser.Roles?.Select(x => Role.Of(x.Name, x.Name)).ToArray();
            var refreshTokens = appUser.RefreshTokens;

            var user = User.Of(new UserId(Guid.Parse(appUser.Id)), appUser.Email, appUser.FirstName, appUser.LastName,
                appUser.Name, appUser.UserName, appUser.PhoneNumber, null!,
                userType, appUser.IsAdministrator, appUser.IsActive,
                appUser.LockoutEnabled, appUser.EmailConfirmed,
                appUser.PhotoUrl, appUser.Status, appUser.CreatedBy, appUser.CreatedDate, appUser.ModifiedBy,
                appUser.ModifiedDate);

            user.AssignPermission(permissions);
            user.AssignRole(roles);
            user.AssignRefreshToken(refreshTokens.ToArray());

            return user;
        }

        public static UserDto ToUserDto(this ApplicationUser appUser)
        {
            if (appUser is null)
                return null;

            var userType = EnumUtility.SafeParse(appUser.UserType, UserType.Customer);

            return new UserDto()
            {
                Id = Guid.Parse(appUser.Id),
                Name = appUser.UserName,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                Permissions = appUser.Permissions.Select(x => x.Name).ToList(),
                Roles = appUser.Roles.Select(x => x.Name).ToList(),
                CreatedDate = appUser.CreatedDate,
                CreatedBy = appUser.CreatedBy,
                EmailConfirmed = appUser.EmailConfirmed,
                IsActive = appUser.IsActive,
                IsAdministrator = appUser.IsAdministrator,
                PhotoUrl = appUser.PhotoUrl,
                UserName = appUser.UserName,
                PhoneNumber = appUser.PhoneNumber,
                UserType = userType,
            };
        }
    }
}