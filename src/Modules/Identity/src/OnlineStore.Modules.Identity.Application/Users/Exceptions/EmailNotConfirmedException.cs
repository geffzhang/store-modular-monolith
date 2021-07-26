using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class EmailNotConfirmedException : AppException
    {
        public string Email { get; }

        public EmailNotConfirmedException(string email) : base($"Email not confirmed for email address `{email}`")
        {
            Email = email;
        }
    }
}