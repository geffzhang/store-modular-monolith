using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class UserCanNotEditException : AppException
    {
        public UserCanNotEditException(string userName) : base($"User '{userName}' can't be edited.")
        {
        }
    }
}