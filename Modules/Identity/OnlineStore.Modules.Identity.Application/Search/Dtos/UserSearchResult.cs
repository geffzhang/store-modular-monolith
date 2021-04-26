using System.Collections.Generic;
using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Search.Dtos
{
    public class UserSearchResult : GenericSearchResult<User>
    {
        public IList<User> Users => Results;
    }
}