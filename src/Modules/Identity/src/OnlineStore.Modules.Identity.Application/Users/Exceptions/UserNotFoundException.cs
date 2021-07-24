using System;
using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(Guid userId) : base($"User with ID: '{userId}' was not found.")
        {
        }
        public UserNotFoundException(string userName) : base($"UserName or Email: '{userName}' was not found.")
        {
        }
    }
}