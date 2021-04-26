using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Domain.Search
{
    public class UserSearchResult : GenericSearchResult<ApplicationUser>
    {
        public IList<ApplicationUser> Users => Results;
    }
}