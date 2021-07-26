using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class UserInActiveException : AppException
    {
        public string UserName { get; }

        public UserInActiveException(string userName) : base($"username {userName} is inactive.")
        {
            UserName = userName;
        }
    }
}