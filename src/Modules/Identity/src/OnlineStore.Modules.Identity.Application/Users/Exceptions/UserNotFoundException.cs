using System;
using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class UserNotFoundException : AppException
    {
        public Guid UserId { get; }
        public string? UserName { get; }

        public UserNotFoundException() : base("User not found.")
        {
        }
        public UserNotFoundException(Guid userId) : base($"User with ID: '{userId}' was not found.")
        {
            UserId = userId;
        }
        public UserNotFoundException(string userName) : base($"UserName or Email: '{userName}' was not found.")
        {
            UserName = userName;
        }
    }
}