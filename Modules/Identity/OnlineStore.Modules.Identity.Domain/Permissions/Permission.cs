using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Common.Domain;
using Common.Domain.Types;
using Common.Utils.Extensions;
using Newtonsoft.Json;

namespace OnlineStore.Modules.Identity.Domain.Permissions
{
    public class Permission : ValueObject
    {
        #region Permissions

        public static Permission ModuleQuery = new(SecurityConstants.Permissions.ModuleQuery, "module");
        public static Permission ModuleAccess = new(SecurityConstants.Permissions.ModuleAccess, "module");
        public static Permission ModuleManage = new(SecurityConstants.Permissions.ModuleManage, "module");

        public static Permission SettingQuery = new(SecurityConstants.Permissions.SecurityQuery, "setting");
        public static Permission SettingAccess = new(SecurityConstants.Permissions.SecurityAccess, "setting");
        public static Permission SettingUpdate = new(SecurityConstants.Permissions.SettingUpdate, "setting");

        // Users Permissions
        public static Permission CanSeeUsersDetail = new(SecurityConstants.Permissions.CanSeeUsersDetail, "user");
        public static Permission CanEditUsers = new(SecurityConstants.Permissions.CanEditUsers, "user");
        public static Permission CanInviteUsers = new(SecurityConstants.Permissions.CanInviteUsers, "user");
        public static Permission CanCreateUsers = new(SecurityConstants.Permissions.CanCreateUsers, "user");
        public static Permission CanDeleteUsers = new(SecurityConstants.Permissions.CanDeleteUsers, "user");
        public static Permission CanViewUsers = new(SecurityConstants.Permissions.CanViewUsers, "user");

        // Admin Permissions
        public static Permission CanSeeAdminsDetail = new(SecurityConstants.Permissions.CanSeeAdminsDetail, "admin");
        public static Permission CanEditAdmins = new(SecurityConstants.Permissions.CanEditAdmins, "admin");
        public static Permission CanCreateAdmins = new(SecurityConstants.Permissions.CanCreateAdmins, "admin");
        public static Permission CanDeleteAdmins = new(SecurityConstants.Permissions.CanDeleteAdmins, "admin");
        public static Permission CanViewAdmins = new(SecurityConstants.Permissions.CanViewAdmins, "admin");

        public static Permission SecurityQuery = new(SecurityConstants.Permissions.SecurityQuery, "security");
        public static Permission SecurityCreate = new(SecurityConstants.Permissions.SecurityCreate, "security");
        public static Permission SecurityAccess = new(SecurityConstants.Permissions.SecurityAccess, "security");
        public static Permission SecurityUpdate = new(SecurityConstants.Permissions.SecurityUpdate, "security");
        public static Permission SecurityDelete = new(SecurityConstants.Permissions.SecurityDelete, "security");

        public static Permission SecurityVerifyEmail =
            new(SecurityConstants.Permissions.SecurityVerifyEmail, "security");

        public static Permission SecurityLoginOnBehalf =
            new(SecurityConstants.Permissions.SecurityLoginOnBehalf, "security");

        #endregion


        private const char _scopeCharSeparator = '|';

        public string Name { get; private set; }

        /// <summary>
        /// Display name of the group to which this permission belongs. The '|' character is used to separate Child and parent groups.
        /// </summary>
        public string GroupName { get; private set; }

        public IList<PermissionScope> AssignedScopes { get; private set; } = new List<PermissionScope>();

        public IList<PermissionScope> AvailableScopes { get; } = new List<PermissionScope>();

        private Permission(string name, string groupName)
        {
            Name = name;
            GroupName = groupName;
        }

        public static IList<Permission> GetAllPermissions()
        {
            return new List<Permission>()
            {
                ModuleQuery,
                ModuleAccess,
                ModuleManage,
                SettingQuery,
                SettingAccess,
                SettingUpdate,
                CanSeeUsersDetail,
                CanEditUsers,
                CanInviteUsers,
                CanCreateUsers,
                CanDeleteUsers,
                CanViewUsers,
                CanSeeAdminsDetail,
                CanEditAdmins,
                CanCreateAdmins,
                CanDeleteAdmins,
                CanViewAdmins,
                SecurityQuery,
                SecurityCreate,
                SecurityAccess,
                SecurityUpdate,
                SecurityDelete,
                SecurityVerifyEmail,
                SecurityLoginOnBehalf
            };
        }

        public static Permission Of(string name, string groupName) => new(name, groupName);

        public static Permission TryCreateFromClaim(Claim claim, JsonSerializerSettings jsonSettings)
        {
            Permission result = null;
            if (claim != null && claim.Type.EqualsInvariant(SecurityConstants.Claims.PermissionClaimType))
            {
                result = AbstractTypeFactory<Permission>.TryCreateInstance();
                result.Name = claim.Value;
                if (result.Name.Contains(_scopeCharSeparator))
                {
                    var parts = claim.Value.Split(_scopeCharSeparator);
                    result.Name = parts.First();
                    result.AssignedScopes =
                        JsonConvert.DeserializeObject<PermissionScope[]>(parts.Skip(1).FirstOrDefault() ?? string.Empty,
                            jsonSettings);
                }
            }

            return result;
        }

        public virtual Claim ToClaim(JsonSerializerSettings jsonSettings)
        {
            var result = Name;
            if (!AssignedScopes.IsNullOrEmpty())
            {
                result += _scopeCharSeparator + JsonConvert.SerializeObject(AssignedScopes, jsonSettings);
            }

            return new Claim(SecurityConstants.Claims.PermissionClaimType, result);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            if (!AssignedScopes.IsNullOrEmpty())
            {
                foreach (var scope in AssignedScopes)
                {
                    yield return scope;
                }
            }
        }

        public virtual void Patch(Permission target)
        {
            target.Name = Name;
            target.GroupName = GroupName;
            if (!AssignedScopes.IsNullOrEmpty())
            {
                AssignedScopes.Patch(target.AssignedScopes,
                    (sourceScope, targetScope) => sourceScope.Patch(targetScope));
            }

            if (!AvailableScopes.IsNullOrEmpty())
            {
                AvailableScopes.Patch(target.AvailableScopes,
                    (sourceScope, targetScope) => sourceScope.Patch(targetScope));
            }
        }
    }
}