using System.Collections.Generic;
using BuildingBlocks.Core.Types;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Types;

namespace OnlineStore.Modules.Identity.Application.Users.Dtos.GatewayResponses.Repositories
{
    public class CreateUserResponse : BaseGatewayResponse
    {
        public UserId Id { get; }
        public CreateUserResponse(UserId id, bool success = false, IEnumerable<Error> errors = null) : base(success, errors)
        {
            Id = id;
        }
    }
}