using System.Collections.Generic;
using System.Linq;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Roles
{
    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole()
        {
            Permissions = new List<Permission>();
        }

        public string Description { get; set; }
        public IList<Permission> Permissions { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        
        public virtual void Patch(ApplicationRole target)
        {
            target.Name = Name;
            target.NormalizedName = NormalizedName;
            target.ConcurrencyStamp = ConcurrencyStamp;
            target.Description = Description;

            if (Permissions.Any()) 
                Permissions.Patch(target.Permissions, (sourcePermission, targetPermission) => sourcePermission.Patch(targetPermission));
        }
    }
}