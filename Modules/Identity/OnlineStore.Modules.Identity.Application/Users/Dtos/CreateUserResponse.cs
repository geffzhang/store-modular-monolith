using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Users.Types;

namespace OnlineStore.Modules.Identity.Application.Users.Dtos
{
    public class CreateUserResponse
    {
        public UserId Id { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}