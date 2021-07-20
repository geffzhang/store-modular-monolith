using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.ResetUserPassword
{
    public class ResetPasswordFailedException : AppException
    {
        public ResetPasswordFailedException(string userId) : base($"An error occured while reset password for userId '{userId}'")
        {
        }
    }
}