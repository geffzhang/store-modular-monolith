using System.Collections.Generic;
using System.Security.Claims;
using Common.Core.Messaging.Queries;

namespace OnlineStore.Modules.Identity.Application.Features.Users.GetUserInfo
{
    public class GetUserInfoQuery: IQuery<IList<Claim>>
    {
    }
}