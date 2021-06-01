using System;
using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Domain.Configurations.Options
{
    public class AuthorizationOptions
    {
        public IEnumerable<string> NonEditableUsers { get; set; }
        public TimeSpan RefreshTokenLifeTime { get; set; }
        public TimeSpan AccessTokenLifeTime { get; set; }
        public string LimitedCookiePermissions { get; set; }
    }
}
