using System.Security.Claims;
using System.Threading.Tasks;
using Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineStore.Modules.Identity.Domain.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users
{
    /// <summary>
    /// Custom UserCalimsPrincipalFactory responds to add claims with system roles based on user properties that can be used for authorization checks
    /// </summary>
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public CustomUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {

        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var result = await base.GenerateClaimsAsync(user);
            var userType = EnumUtility.SafeParse(user.UserType, UserType.Customer);
            //need to transform isAdministrator flag and user types into special system roles claims
            if (user.IsAdministrator)
            {
                result.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, SecurityConstants.SystemRoles.Administrator));
            }
            else if (userType == UserType.Customer)
            {
                result.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, SecurityConstants.SystemRoles.Customer));
            }
            else if (userType == UserType.Manager)
            {
                result.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, SecurityConstants.SystemRoles.Manager));
            }
            return result;
        }
    }
}
