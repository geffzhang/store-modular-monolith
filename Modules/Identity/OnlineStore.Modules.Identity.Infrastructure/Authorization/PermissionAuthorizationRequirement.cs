using Microsoft.AspNetCore.Authorization;

namespace OnlineStore.Modules.Identity.Infrastructure.Authorization
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
