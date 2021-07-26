using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Features.ConfirmEmail
{
    public class ConfirmationEmailFailedException : AppException
    {
        public ConfirmationEmailFailedException(string userId) : base(
            $"Confirmation email failed for UserId '{userId}'")
        {
        }
    }
}