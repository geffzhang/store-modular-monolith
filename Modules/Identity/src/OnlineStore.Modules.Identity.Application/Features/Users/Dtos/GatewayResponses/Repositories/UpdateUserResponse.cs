using System.Collections.Generic;
using Common.Domain.Types;
using Common.Types;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Dtos.GatewayResponses.Repositories
{
    public class UpdateUserResponse: BaseGatewayResponse
    {
        public User User { get; }
        public UpdateUserResponse(User user, bool success = false, IEnumerable<Error> errors = null) : base(success, errors)
        {
            User = user;
        }
    }
}