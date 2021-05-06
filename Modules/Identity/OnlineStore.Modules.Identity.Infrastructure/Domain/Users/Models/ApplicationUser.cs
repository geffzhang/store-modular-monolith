using System;
using System.Collections.Generic;
using Common.Domain.Types;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models
{
    public class ApplicationUser : IdentityUser<string>, IAuditable
    {
        public ApplicationUser()
        {
        }

        public virtual bool IsActive { get; set; }
        public virtual string MemberId { get; set; }
        public virtual bool IsAdministrator { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Name { get; set; }
        public virtual string PhotoUrl { get; set; }
        public virtual string UserType { get; set; }
        public virtual string Status { get; set; }

        [Obsolete(
            "Left due to compatibility issues. Will be removed. Instead of, use properties: EmailConfirmed, LockoutEnd.")]
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual AccountState UserState { get; set; }

        public virtual string Password { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual IList<ApplicationRole> Roles { get; set; }
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }
        public virtual string[] Permissions { get; set; }

        /// <summary>
        /// External provider logins.
        /// </summary>
        public virtual ApplicationUserLogin[] Logins { get; set; }

        /// <summary>
        /// Indicates that the password for this user is expired and must be changed.
        /// </summary>
        public virtual bool PasswordExpired { get; set; }

        /// <summary>
        /// The last date when the password was changed
        /// </summary>
        public virtual DateTime? LastPasswordChangedDate { get; set; }


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
            target.MemberId = MemberId;
            target.PhotoUrl = PhotoUrl;
            target.UserType = UserType;
            target.Status = Status;
            target.Password = Password;
            target.PasswordExpired = PasswordExpired;
            target.LastPasswordChangedDate = LastPasswordChangedDate;
        }
    }
}