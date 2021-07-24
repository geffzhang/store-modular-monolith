using System.Collections.Generic;
using OnlineStore.Modules.Identity.Application.Permissions.Services;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Services
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
