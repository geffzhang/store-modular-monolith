using System.Collections.Generic;
using System.Security.Claims;
using Common.Messaging.Queries;

namespace OnlineStore.Modules.Identity.Application.Users.GetUserInfo
{
    public class GetUserInfoQuery: IQuery<IList<Claim>>
    {
    }
}