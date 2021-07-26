using System.Collections.Generic;
using BuildingBlocks.Core.Types;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.Features.SearchUsers
{
    public class UserSearchResponse : GenericSearchResult<UserDto>
    {
        public IList<UserDto> Users => Results;
    }
}