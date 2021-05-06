using System.Collections.Generic;
using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Role;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Search.Dtos
{
    public class RoleSearchResult : GenericSearchResult<Role>
    {
        public IList<Role> Roles => Results;
    }
}