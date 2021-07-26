using System.Collections.Generic;
using System.Security.Claims;
using BuildingBlocks.Cqrs.Queries;

namespace OnlineStore.Modules.Identity.Application.Users.Features.GetUserInfo
{
    public class GetUserInfoQuery: IQuery<IList<Claim>>
    {
    }
}