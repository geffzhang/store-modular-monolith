using System;
using System.Security.Claims;
using Common.Core.Extensions;
using Newtonsoft.Json;
using OnlineStore.Modules.Identity.Domain;
using OnlineStore.Modules.Identity.Domain.Constants;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Features.Users
{
    /// <summary>
    /// https://www.jerriepelser.com/blog/useful-claimsprincipal-extension-methods/
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Email);
        }

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public static bool IsCurrentUser(this ClaimsPrincipal principal, string id)
        {
            var currentUserId = GetUserId(principal);

            return string.Equals(currentUserId, id, StringComparison.OrdinalIgnoreCase);
        }

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
                result = principal.HasClaim(SecurityConstants.Claims.PermissionClaimType, permissionName);
            }
            return result;
        }
    }
}