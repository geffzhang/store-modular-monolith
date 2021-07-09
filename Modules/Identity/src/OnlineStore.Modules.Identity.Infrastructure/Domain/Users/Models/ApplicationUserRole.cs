using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}