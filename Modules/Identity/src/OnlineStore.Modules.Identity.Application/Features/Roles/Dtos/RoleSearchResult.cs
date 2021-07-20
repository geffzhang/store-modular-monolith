using System.Collections.Generic;
using Common.Core.Types;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Features.Roles.Dtos
{
    public class RoleSearchResult : GenericSearchResult<Role>
    {
        public IList<Role> Roles => Results;
    }
}