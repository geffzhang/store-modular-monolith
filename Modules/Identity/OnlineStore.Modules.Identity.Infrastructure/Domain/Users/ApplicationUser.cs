using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Identity
{
    public class ApplicationUser : IdentityUser<string>
    {
        public ApplicationUser()
        {
        }

        /// <summary>
        /// Tenant id
        /// </summary>
        public virtual string StoreId { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual string MemberId { get; set; }
        public virtual bool IsAdministrator { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Name { get; set; }
        public virtual string PhotoUrl { get; set; }
        public virtual string UserType { get; set; }
        public virtual string Status { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual IList<ApplicationRole> Roles { get; set; }

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
    }
}