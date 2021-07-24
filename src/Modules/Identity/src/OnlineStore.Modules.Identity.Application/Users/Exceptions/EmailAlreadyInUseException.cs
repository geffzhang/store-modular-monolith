using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
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