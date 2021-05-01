using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Permissions;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions
{
    public interface IPermissionService
    {
        void RegisterPermissions(Permission[] permissions);
        IEnumerable<Permission> GetAllPermissions();
    }
}