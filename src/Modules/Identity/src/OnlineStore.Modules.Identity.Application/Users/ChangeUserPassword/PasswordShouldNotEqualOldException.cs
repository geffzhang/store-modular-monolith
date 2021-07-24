using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.ChangeUserPassword
{
    public class PasswordShouldNotEqualOldException : AppException
    {
        public PasswordShouldNotEqualOldException() : base(
            "You have used this password in the past. Choose another one.")
        {
        }
    }
}