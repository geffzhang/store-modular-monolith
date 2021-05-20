using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Common.Domain.Types;
using Common.Utils.Extensions;
using Newtonsoft.Json;

namespace OnlineStore.Modules.Identity.Domain.Users
{
    public sealed class Permission : ValueObject
    {
        #region Permissions

        public static readonly Permission ModuleQuery = new(SecurityConstants.Permissions.ModuleQuery, "module");
        public static readonly Permission ModuleAccess = new(SecurityConstants.Permissions.ModuleAccess, "module");
        public static readonly Permission ModuleManage = new(SecurityConstants.Permissions.ModuleManage, "module");

        public static readonly Permission SettingQuery = new(SecurityConstants.Permissions.SecurityQuery, "setting");
        public static readonly Permission SettingAccess = new(SecurityConstants.Permissions.SecurityAccess, "setting");
        public static readonly Permission SettingUpdate = new(SecurityConstants.Permissions.SettingUpdate, "setting");

        // Users Permissions
        public static readonly Permission SeeUsersDetail =
            new(SecurityConstants.Permissions.SeeUsersDetail, "user");

        public static readonly Permission EditUsers = new(SecurityConstants.Permissions.EditUsers, "user");
        public static readonly Permission InviteUsers = new(SecurityConstants.Permissions.InviteUsers, "user");
        public static readonly Permission CreateUsers = new(SecurityConstants.Permissions.CreateUsers, "user");
        public static readonly Permission DeleteUsers = new(SecurityConstants.Permissions.DeleteUsers, "user");
        public static readonly Permission ViewUsers = new(SecurityConstants.Permissions.ViewUsers, "user");

        // Admin Permissions
        public static readonly Permission SeeAdminsDetail =
            new(SecurityConstants.Permissions.SeeAdminsDetail, "admin");

        public static readonly Permission EditAdmins = new(SecurityConstants.Permissions.EditAdmins, "admin");

        public static readonly Permission CreateAdmins =
            new(SecurityConstants.Permissions.CreateAdmins, "admin");

        public static readonly Permission DeleteAdmins =
            new(SecurityConstants.Permissions.DeleteAdmins, "admin");

        public static readonly Permission ViewAdmins = new(SecurityConstants.Permissions.ViewAdmins, "admin");

        public static readonly Permission SecurityQuery = new(SecurityConstants.Permissions.SecurityQuery, "security");

        public static readonly Permission SecurityCreate =
            new(SecurityConstants.Permissions.SecurityCreate, "security");

        public static readonly Permission SecurityAccess =
            new(SecurityConstants.Permissions.SecurityAccess, "security");

        public static readonly Permission SecurityUpdate =
            new(SecurityConstants.Permissions.SecurityUpdate, "security");

        public static readonly Permission SecurityDelete =
            new(SecurityConstants.Permissions.SecurityDelete, "security");

        public static readonly Permission SecurityVerifyEmail =
            new(SecurityConstants.Permissions.SecurityVerifyEmail, "security");

        public static readonly Permission SecurityLoginOnBehalf =
            new(SecurityConstants.Permissions.SecurityLoginOnBehalf, "security");

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
                SeeUsersDetail,
                EditUsers,
                InviteUsers,
                CreateUsers,
                DeleteUsers,
                ViewUsers,
                SeeAdminsDetail,
                EditAdmins,
                CreateAdmins,
                DeleteAdmins,
                ViewAdmins,
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
            if (claim != null && claim.Type.EqualsInvariant(SecurityConstants.Claims.PermissionClaimType))
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