using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class EmailAlreadyInUsedException : AppException
    {
        public string Email { get; }

        public EmailAlreadyInUsedException(string email) : base($"Email '{email}' is already in use.")
        {
            Email = email;
        }
    }
}