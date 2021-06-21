using System;
using System.Linq;
using Common.Utils.Extensions;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses;
using OnlineStore.Modules.Identity.Application.Features.Users.Services;
using OnlineStore.Modules.Identity.Domain.Configurations.Options;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Domain.Users.Services;
using OnlineStore.Modules.Identity.Domain.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles.Mappings;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Mappings
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
                UserType = user.UserType.ToString(),
                CreatedBy = user.CreatedBy,
                CreatedDate = user.CreatedDate,
                ModifiedBy = user.ModifiedBy,
                ModifiedDate = user.ModifiedDate
            };

            return applicationUser;
        }

        public static User ToUser(this ApplicationUser appUser, IUserEditable userEditable = null)
        {
            if (appUser is null)
                return null;
            
            var userType = EnumUtility.SafeParse(appUser.UserType, UserType.Customer);

            return User.Of(new UserId(Guid.Parse(appUser.Id)), appUser.Email, appUser.FirstName, appUser.LastName,
                appUser.Name, appUser.UserName, null!,
                appUser.Permissions?.Select(x => x.Name).ToList()!, userType, userEditable!, appUser.IsAdministrator, appUser.IsActive,
                appUser.Roles?.Select(x => x.Name).ToList()!, appUser.LockoutEnabled, appUser.EmailConfirmed,
                appUser.PhotoUrl, appUser.Status, appUser.CreatedBy, appUser.CreatedDate, appUser.ModifiedBy, appUser.ModifiedDate);
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
                UserType = userType,
            };
        }
    }
}