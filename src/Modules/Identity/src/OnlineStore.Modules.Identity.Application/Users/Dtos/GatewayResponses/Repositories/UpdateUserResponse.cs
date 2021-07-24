using System.Collections.Generic;
using BuildingBlocks.Core.Types;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;

namespace OnlineStore.Modules.Identity.Application.Users.Dtos.GatewayResponses.Repositories
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