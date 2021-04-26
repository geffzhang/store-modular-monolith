using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users
{
    public interface IHasSecurityAccounts 
    {
        /// <summary>
        /// All security accounts 
        /// </summary>
        ICollection<ApplicationUser> SecurityAccounts { get; set; }
    }
}
