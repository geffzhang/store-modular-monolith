using System.Collections.Generic;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Application.Users.Dtos
{
    public class UserSearchResponse : GenericSearchResult<UserDto>
    {
        public IList<UserDto> Users => Results;
    }
}