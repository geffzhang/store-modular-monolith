using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Domain.Search
{
    public class RoleSearchResult : GenericSearchResult<Role>
    {
        public IList<ApplicationRole> Roles => Results;
    }
}