using System.Linq;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Roles
{
    public static class MappingExtensions
    {
        public static ApplicationRole ToApplicationRole(this Role role)
        {
            return new()
            {
                Description = role.Description, Id = role.Name, Name = role.Name, Permissions = role.Permissions
            };
        }

        public static Role ToRole(this ApplicationRole role)
        {
            return Role.Of(role.Name, role.Id, role.Description, role.Permissions.ToArray());
        }
    }
}