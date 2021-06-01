using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.ChangeUserPassword
{
    public class PasswordShouldNotEqualOldException : AppException
    {
        public PasswordShouldNotEqualOldException() : base(
            "You have used this password in the past. Choose another one.")
        {
        }
    }
}