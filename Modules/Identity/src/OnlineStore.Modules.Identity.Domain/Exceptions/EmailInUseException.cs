using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Exceptions
{
    internal class EmailInUseException : DomainException
    {
        public EmailInUseException(string email) : base($"Email {email} is already in use.")
        {
            Email = email;
        }

        public string Email { get; }
    }
}