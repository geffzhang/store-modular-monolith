using System.Collections.Generic;
using Common.Core.Types;
using OnlineStore.Modules.Identity.Domain.Users.Types;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Dtos.GatewayResponses.Repositories
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