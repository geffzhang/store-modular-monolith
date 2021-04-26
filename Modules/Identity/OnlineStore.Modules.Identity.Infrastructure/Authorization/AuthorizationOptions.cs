using System;
using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Infrastructure.Authorization
{
    public class AuthorizationOptions
    {
        public IEnumerable<string> NonEditableUsers { get; set; }
        public TimeSpan RefreshTokenLifeTime { get; set; }
        public TimeSpan AccessTokenLifeTime { get; set; }

        // LimitedPermissions claims that will be granted to the user by cookies when bearer token authentication is enabled.
        //
        // If the user identity has claim named "limited_permissions", this attribute should authorize only permissions listed in that claim. Any permissions that are required by this attribute but
        // not listed in the claim should cause this method to return false. However, if permission limits of user identity are not defined ("limited_permissions" claim is missing),
        // then no limitations should be applied to the permissions.
        public string LimitedCookiePermissions { get; set; }
    }
}
