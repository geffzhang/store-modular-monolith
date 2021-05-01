using System;
using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Users.Dtos
{
    public class CreateUserResponse
    {
        public UserId Id { get; }
        public bool Success { get; }
        public IEnumerable<string> Errors { get; }

        public CreateUserResponse(UserId id, bool success = false, IEnumerable<string> errors = null)
        {
            Id = id;
            Success = success;
            Errors = errors;
        }
    }
}