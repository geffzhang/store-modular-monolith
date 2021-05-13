using System;
using System.Linq;
using Common.Utils.Extensions;
using OnlineStore.Modules.Identity.Application.Users.Dtos;
using OnlineStore.Modules.Identity.Application.Users.RegisterNewUser;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Domain.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles.Mappings;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Mappings
{
    public static class MappingExtensions
    {
        public static ApplicationUser ToApplicationUser(this User user)
        {
            var applicationUser = new ApplicationUser
            {
                Name = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Id = user.Id.Id.ToString(),
                Permissions = user.Permissions.ToList(),
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
                UserType = user.UserType.ToString(),
            };

            return applicationUser;
        }

        public static User ToUser(this ApplicationUser appUser)
        {
            var userType = EnumUtility.SafeParse(appUser.UserType, UserType.Customer);

            return User.Of(new UserId(Guid.Parse(appUser.Id)), appUser.Email, appUser.FirstName, appUser.LastName,
                appUser.Name, appUser.UserName, appUser.Password, appUser.CreatedDate, appUser.CreatedBy,
                appUser.Permissions.Select(x => x.Name).ToList(), userType, appUser.IsAdministrator, appUser.IsActive,
                appUser.Roles.Select(x => x.Name).ToList(), appUser.LockoutEnabled, appUser.EmailConfirmed,
                appUser.PhotoUrl, appUser.Status, appUser.ModifiedBy, appUser.ModifiedDate);
        }
        public static UserDto ToUserDto(this ApplicationUser appUser)
        {
            var userType = EnumUtility.SafeParse(appUser.UserType, UserType.Customer);

            return new UserDto()
                {
                    Id = Guid.Parse(appUser.Id),
                    Name = appUser.UserName,
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    Email = appUser.Email,
                    Password = appUser.Password,
                    Permissions = appUser.Permissions.Select(x => x.Name).ToList(),
                    Roles = appUser.Roles.Select(x => x.Name).ToList(),
                    CreatedDate = appUser.CreatedDate,
                    CreatedBy = appUser.CreatedBy,
                    EmailConfirmed = appUser.EmailConfirmed,
                    IsActive = appUser.IsActive,
                    IsAdministrator = appUser.IsAdministrator,
                    MemberId = appUser.MemberId,
                    PhotoUrl = appUser.PhotoUrl,
                    UserName = appUser.UserName,
                    UserType = userType,
                };
        }
    }
}