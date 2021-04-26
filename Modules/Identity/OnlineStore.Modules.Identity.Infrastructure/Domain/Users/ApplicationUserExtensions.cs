using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users
{
    public static class ApplicationUserExtensions
    {
        public static Dictionary<string, string> DetectUserChanges(this ApplicationUser newUser,
            ApplicationUser oldUser)
        {
            var result = new Dictionary<string, string>();

            if (newUser.UserName != oldUser.UserName)
            {
                result.Add(SecurityConstants.Changes.UserUpdated,
                    $"Changes: user name: {oldUser.UserName} -> {newUser.UserName}");
            }

            if (newUser.Email != oldUser.Email)
            {
                result.Add(SecurityConstants.Changes.UserUpdated,
                    $"Changes: email: {oldUser.Email} -> {newUser.Email}");
            }

            if (newUser.UserType != oldUser.UserType)
            {
                result.Add(SecurityConstants.Changes.UserUpdated,
                    $"Changes: user type: {oldUser.UserType} -> {newUser.UserType}");
            }

            if (newUser.IsAdministrator != oldUser.IsAdministrator)
            {
                result.Add(SecurityConstants.Changes.UserUpdated,
                    $"Changes: root: {oldUser.IsAdministrator} -> {newUser.IsAdministrator}");
            }

            if (newUser.MemberId != oldUser.MemberId)
            {
                result.Add(SecurityConstants.Changes.UserUpdated,
                    $"Changes: member ID: {oldUser.MemberId} -> {newUser.MemberId}");
            }

            if (newUser.PasswordHash != oldUser.PasswordHash)
            {
                result.Add(SecurityConstants.Changes.UserPasswordChanged, $"Password changed");
            }

            return result;
        }
    }
}