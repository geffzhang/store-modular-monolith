using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Application.Features.Roles.Dtos
{
    public class CreateRoleResponse
    {
        public string Id { get; }
        public bool Success { get; }
        public IEnumerable<string> Errors { get; }

        public CreateRoleResponse(string id, bool success = false, IEnumerable<string> errors = null)
        {
            Id = id;
            Success = success;
            Errors = errors;
        }
    }
}