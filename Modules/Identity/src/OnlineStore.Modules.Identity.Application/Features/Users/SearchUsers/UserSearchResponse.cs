using System.Collections.Generic;
using Common.Core.Types;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Features.Users.SearchUsers
{
    public class UserSearchResponse : GenericSearchResult<UserDto>
    {
        public IList<UserDto> Users => Results;
    }
}