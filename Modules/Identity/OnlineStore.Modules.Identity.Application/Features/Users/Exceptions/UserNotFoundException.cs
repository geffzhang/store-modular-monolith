using System;
using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Exceptions
{
    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(Guid userId) : base($"User with ID: '{userId}' was not found.")
        {
        }
        public UserNotFoundException(string userName) : base($"UserName: '{userName}' was not found.")
        {
        }
    }
}