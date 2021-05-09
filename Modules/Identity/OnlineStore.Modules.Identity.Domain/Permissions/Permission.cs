using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Common.Domain;
using Common.Domain.Types;
using Common.Utils.Extensions;
using Common.Utils.Reflection;
using Newtonsoft.Json;

namespace OnlineStore.Modules.Identity.Domain.Permissions
{
    public sealed class Permission : ValueObject
    {
        #region Permissions

        public static readonly Permission ModuleQuery = new(SecurityConstants.Permissions.MODULE_QUERY, "module");
        public static readonly Permission ModuleAccess = new(SecurityConstants.Permissions.MODULE_ACCESS, "module");
        public static readonly Permission ModuleManage = new(SecurityConstants.Permissions.MODULE_MANAGE, "module");

        public static readonly Permission SettingQuery = new(SecurityConstants.Permissions.SECURITY_QUERY, "setting");
        public static readonly Permission SettingAccess = new(SecurityConstants.Permissions.SECURITY_ACCESS, "setting");
        public static readonly Permission SettingUpdate = new(SecurityConstants.Permissions.SETTING_UPDATE, "setting");

        // Users Permissions
        public static readonly Permission CanSeeUsersDetail =
            new(SecurityConstants.Permissions.CAN_SEE_USERS_DETAIL, "user");

        public static readonly Permission CanEditUsers = new(SecurityConstants.Permissions.CAN_EDIT_USERS, "user");
        public static readonly Permission CanInviteUsers = new(SecurityConstants.Permissions.CAN_INVITE_USERS, "user");
        public static readonly Permission CanCreateUsers = new(SecurityConstants.Permissions.CAN_CREATE_USERS, "user");
        public static readonly Permission CanDeleteUsers = new(SecurityConstants.Permissions.CAN_DELETE_USERS, "user");
        public static readonly Permission CanViewUsers = new(SecurityConstants.Permissions.CAN_VIEW_USERS, "user");

        // Admin Permissions
        public static readonly Permission CanSeeAdminsDetail =
            new(SecurityConstants.Permissions.CAN_SEE_ADMINS_DETAIL, "admin");

        public static readonly Permission CanEditAdmins = new(SecurityConstants.Permissions.CAN_EDIT_ADMINS, "admin");

        public static readonly Permission CanCreateAdmins =
            new(SecurityConstants.Permissions.CAN_CREATE_ADMINS, "admin");

        public static readonly Permission CanDeleteAdmins =
            new(SecurityConstants.Permissions.CAN_DELETE_ADMINS, "admin");

        public static readonly Permission CanViewAdmins = new(SecurityConstants.Permissions.CAN_VIEW_ADMINS, "admin");

        public static readonly Permission SecurityQuery = new(SecurityConstants.Permissions.SECURITY_QUERY, "security");

        public static readonly Permission SecurityCreate =
            new(SecurityConstants.Permissions.SECURITY_CREATE, "security");

        public static readonly Permission SecurityAccess =
            new(SecurityConstants.Permissions.SECURITY_ACCESS, "security");

        public static readonly Permission SecurityUpdate =
            new(SecurityConstants.Permissions.SECURITY_UPDATE, "security");

        public static readonly Permission SecurityDelete =
            new(SecurityConstants.Permissions.SECURITY_DELETE, "security");

        public static readonly Permission SecurityVerifyEmail =
            new(SecurityConstants.Permissions.SECURITY_VERIFY_EMAIL, "security");

        public static readonly Permission SecurityLoginOnBehalf =
            new(SecurityConstants.Permissions.SECURITY_LOGIN_ON_BEHALF, "security");

        #endregion


        private const char SCOPE_CHAR_SEPARATOR = '|';

        public string Name { get; private set; }

        /// <summary>
        /// Display name of the group to which this permission belongs. The '|' character is used to separate Child and parent groups.
        /// </summary>
        public string GroupName { get; private set; }

        public IList<PermissionScope> AssignedScopes { get; private set; } = new List<PermissionScope>();

        public IList<PermissionScope> AvailableScopes { get; } = new List<PermissionScope>();

        private Permission(string name, string groupName = null)
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

        public static Permission Of(string name, string groupName = null)
        {
            return new(name, groupName);
        }

        public static Permission TryCreateFromClaim(Claim claim, JsonSerializerSettings jsonSettings)
        {
            Permission result = null;
            if (claim != null && claim.Type.EqualsInvariant(SecurityConstants.Claims.PERMISSION_CLAIM_TYPE))
            {
                result = new(claim.Value);
                if (result.Name.Contains(SCOPE_CHAR_SEPARATOR))
                {
                    var parts = claim.Value.Split(SCOPE_CHAR_SEPARATOR);
                    result.Name = parts.First();
                    result.AssignedScopes =
                        JsonConvert.DeserializeObject<PermissionScope[]>(parts.Skip(1).FirstOrDefault() ?? string.Empty,
                            jsonSettings);
                }
            }

            return result;
        }

        public Claim ToClaim(JsonSerializerSettings jsonSettings)
        {
            var result = Name;
            if (!AssignedScopes.IsNullOrEmpty())
            {
                result += SCOPE_CHAR_SEPARATOR + JsonConvert.SerializeObject(AssignedScopes, jsonSettings);
            }

            return new Claim(SecurityConstants.Claims.PERMISSION_CLAIM_TYPE, result);
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

        public void Patch(Permission target)
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