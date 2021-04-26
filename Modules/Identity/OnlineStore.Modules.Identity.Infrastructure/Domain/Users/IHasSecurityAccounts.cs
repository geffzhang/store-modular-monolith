using System.Collections.Generic;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;

namespace Common.Identity
{
    public interface IHasSecurityAccounts 
    {
        /// <summary>
        /// All security accounts 
        /// </summary>
        ICollection<ApplicationUser> SecurityAccounts { get; set; }
    }
}
