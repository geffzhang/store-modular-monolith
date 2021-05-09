using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Modules.Identity.Infrastructure.Extensions;

namespace OnlineStore.Modules.Identity.Infrastructure.Authorization
{
    public abstract class PermissionAuthorizationHandlerBase<TRequirement> : AuthorizationHandler<TRequirement>
        where TRequirement : PermissionAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
        {
            var limitedPermissionsClaim =
                context.User.FindFirstValue(Identity.Domain.SecurityConstants.Claims.LIMITED_PERMISSIONS_CLAIM_TYPE);

            // LimitedPermissions claims that will be granted to the user by cookies when bearer token authentication is enabled.
            //
            // If the user identity has claim named "limited_permissions", this attribute should authorize only permissions listed in that claim. Any permissions that are required by this attribute but
            // not listed in the claim should cause this method to return false. However, if permission limits of user identity are not defined ("limited_permissions" claim is missing),
            // then no limitations should be applied to the permissions.
            if (limitedPermissionsClaim != null)
            {
                var limitedPermissions =
                    limitedPermissionsClaim.Split(Identity.Domain.SecurityConstants.Claims.PERMISSION_CLAIM_TYPE_DELIMITER,
                        StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

                if (limitedPermissions.Contains(requirement.Permission))
                {
                    context.Succeed(requirement);
                }
            }
            else
            {
                if (context.User.HasGlobalPermission(requirement.Permission))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}