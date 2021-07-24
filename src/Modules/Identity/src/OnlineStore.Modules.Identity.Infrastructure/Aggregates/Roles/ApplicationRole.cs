using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Core.Extensions;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Roles
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