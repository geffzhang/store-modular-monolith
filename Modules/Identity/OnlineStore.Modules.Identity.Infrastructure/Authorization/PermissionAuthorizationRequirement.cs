using Microsoft.AspNetCore.Authorization;

namespace Common.Identity.Authorization
{
    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        public PermissionAuthorizationRequirement(string permission)
        {
            Permission = permission;
        }
        public string Permission { get; set; }
    }
}
