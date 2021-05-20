using System;
using System.Linq;
using Common.Utils.Extensions;
using OnlineStore.Modules.Identity.Application.Permissions.Services;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Permissions
{
    public static class PermissionServiceExtensions
    {
        public static IPermissionService WithAvailableScopesForPermission(this IPermissionService permissionService,
            string permissionName, params PermissionScope[] scopes)
        {
            return permissionService.WithAvailableScopesForPermissions(new[] {permissionName}, scopes);
        }

        public static IPermissionService WithAvailableScopesForPermissions(this IPermissionService permissionService,
            string[] permissionNames, params PermissionScope[] scopes)
        {
            if (permissionService == null)
            {
                throw new ArgumentNullException(nameof(permissionService));
            }

            if (permissionNames == null)
            {
                throw new ArgumentNullException(nameof(permissionNames));
            }

            var permissions = permissionService.GetAllPermissions().Where(x => permissionNames.Contains(x.Name));
            foreach (var permission in permissions)
            {
                permission.AvailableScopes.AddRange(scopes.Distinct());
            }

            return permissionService;
        }
    }
}