using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.ConfirmEmail
{
    public class ConfirmationEmailFailedException : AppException
    {
        public ConfirmationEmailFailedException(string userId) : base(
            $"Confirmation email failed for UserId '{userId}'")
        {
        }
    }
}