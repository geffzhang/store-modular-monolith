using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Features.Permissions.Services
{
    public interface IPermissionService
    {
        void RegisterPermissions(Permission[] permissions);
        IEnumerable<Permission> GetAllPermissions();
    }
}