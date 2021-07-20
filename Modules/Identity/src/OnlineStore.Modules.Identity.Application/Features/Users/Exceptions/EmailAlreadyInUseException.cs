using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Exceptions
{
    public class EmailAlreadyInUseException : AppException
    {
        public string Email { get; }

        public EmailAlreadyInUseException(string email) : base($"Email '{email}' is already in use.")
        {
            Email = email;
        }
    }
}