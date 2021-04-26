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
    public class Permission : ValueObject, ICloneable
    {
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


        public object Clone()
        {
            var result = MemberwiseClone() as Permission;

            result.AssignedScopes = AssignedScopes?.Select(x => x.Clone()).OfType<PermissionScope>().ToList();

            return result;
        }
    }
}