using System.Collections.Generic;
using BuildingBlocks.Core.Types;

namespace OnlineStore.Modules.Identity.Application.Users.Dtos.GatewayResponses.Repositories
{
    public class SetUserEmailResponse: BaseGatewayResponse
    {
        public string Email { get; }
        public SetUserEmailResponse(string email, bool success = false, IEnumerable<Error> errors = null) : base(success, errors)
        {
            Email = email;
        }
    }
}