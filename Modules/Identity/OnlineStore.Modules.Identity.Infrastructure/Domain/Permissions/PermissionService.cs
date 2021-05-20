using System.Collections.Generic;
using OnlineStore.Modules.Identity.Application.Features.Permissions.Services;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly List<Permission> _permissions = new();

        public IEnumerable<Permission> GetAllPermissions()
        {
            return _permissions;
        }

        public void RegisterPermissions(params Permission[] permissions)
        {
            _permissions.AddRange(permissions);
        }
    }
}
