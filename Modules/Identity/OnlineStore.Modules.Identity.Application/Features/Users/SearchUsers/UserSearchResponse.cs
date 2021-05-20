using System.Collections.Generic;
using Common.Domain.Types;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.SearchUsers
{
    public class UserSearchResponse : GenericSearchResult<UserDto>
    {
        public IList<UserDto> Users => Results;
    }
}