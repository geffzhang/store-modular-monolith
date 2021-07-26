using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Authentication.Features.Login
{
    public class LoginFailedException : AppException
    {
        public string UserNameOrEmail { get;}
        public LoginFailedException(string userNameOrEmail) : base($"Login failed for username: {userNameOrEmail}")
        {
            UserNameOrEmail = userNameOrEmail;
        }
    }
}