using System;
using System.Linq;
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
            };

            return applicationUser;
        }

        public static User ToUser(this ApplicationUser appUser)
        {
            return User.Of(new UserId(Guid.Parse(appUser.Id)), appUser.Email, appUser.FirstName, appUser.LastName,
                appUser.Name, appUser.UserName, appUser.Password, appUser.CreatedDate, appUser.CreatedBy,
                appUser.Permissions, appUser.UserType, appUser.IsAdministrator, appUser.IsActive,
                appUser.Roles.Select(x => x.Name).ToList(), appUser.LockoutEnabled, appUser.EmailConfirmed,
                appUser.PhotoUrl, appUser.Status, appUser.ModifiedBy, appUser.ModifiedDate);
        }

        public static RegisterNewUserCommand ToRegisterNewUserCommand(this RegisterNewUserRequest request)
        {
            var command = new RegisterNewUserCommand(request.Id, request.Email, request.FirstName, request.LastName,
                request.Name, request.UserName, request.Password, request.CreatedDate, request.CreatedBy,
                request.Permissions.ToList(), request.UserType, request.IsAdministrator, request.IsActive,
                request.Roles.ToList(), request.LockoutEnabled, request.EmailConfirmed, request.PhotoUrl,
                request.Status, request.ModifiedBy, request.ModifiedDate);

            return command;
        }

    }
}