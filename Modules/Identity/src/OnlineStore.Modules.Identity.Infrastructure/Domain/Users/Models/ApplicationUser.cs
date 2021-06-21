using System;
using System.Collections.Generic;
using Common.Domain.Types;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models
{
    public class ApplicationUser : IdentityUser<string>, IAuditable
    {
        public bool IsActive { get; set; }
        public bool IsAdministrator { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string UserType { get; set; }
        public string Status { get; set; }
        public AccountState UserState { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool PasswordExpired { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public IList<Permission> Permissions { get; set; }
        public IList<ApplicationRole> Roles { get; set; }

        // .Net identity navigations -> https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model#add-navigation-properties
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }

        /// <summary>
        /// External provider logins.
        /// </summary>
        public virtual ApplicationUserLogin[] Logins { get; set; }

        public virtual void Patch(ApplicationUser target)
        {
            target.UserName = UserName;
            target.IsAdministrator = IsAdministrator;
            target.Email = Email;
            target.NormalizedEmail = NormalizedEmail;
            target.NormalizedUserName = NormalizedUserName;
            target.EmailConfirmed = EmailConfirmed;
            target.PasswordHash = PasswordHash;
            target.SecurityStamp = SecurityStamp;
            target.PhoneNumberConfirmed = PhoneNumberConfirmed;
            target.PhoneNumber = PhoneNumber;
            target.TwoFactorEnabled = TwoFactorEnabled;
            target.LockoutEnabled = LockoutEnabled;
            target.LockoutEnd = LockoutEnd;
            target.AccessFailedCount = AccessFailedCount;
            target.PhotoUrl = PhotoUrl;
            target.UserType = UserType;
            target.Status = Status;
        }
    }
}