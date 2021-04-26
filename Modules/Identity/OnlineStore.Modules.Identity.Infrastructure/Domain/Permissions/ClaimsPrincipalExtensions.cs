using System.Security.Claims;
using Common.Utils.Extensions;
using Newtonsoft.Json;
using OnlineStore.Modules.Identity.Domain.Permissions;

namespace Common.Identity
{
    public static class ClaimsPrincipalExtensions
    {
        public static Permission FindPermission(this ClaimsPrincipal principal, string permissionName, JsonSerializerSettings jsonSettings)
        {
            foreach (var claim in principal.Claims)
            {
                var permission = Permission.TryCreateFromClaim(claim, jsonSettings);
                if (permission != null && permission.Name.EqualsInvariant(permissionName))
                {
                    return permission;
                }
            }
            return null;
        }

        public static bool HasGlobalPermission(this ClaimsPrincipal principal, string permissionName)
        {
            // TODO: Check cases with locked user
            var result = principal.IsInRole(SecurityConstants.SystemRoles.Administrator);

            if (!result)
            {
                // Breaking change in v3:
                // Do not allow users with Customer role login into platform
                result = !principal.IsInRole(SecurityConstants.SystemRoles.Customer);
                if (result)
                {
                    result = principal.HasClaim(SecurityConstants.Claims.PermissionClaimType, permissionName);
                }
            }
            return result;
        }
    }
}
