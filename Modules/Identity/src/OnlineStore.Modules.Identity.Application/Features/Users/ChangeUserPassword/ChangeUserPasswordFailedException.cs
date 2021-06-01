using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.ChangeUserPassword
{
    public class ChangeUserPasswordFailedException : AppException
    {
        public ChangeUserPasswordFailedException(string userId) : base($"An error occured while changing password for userId '{userId}'")
        {
        }
    }
}