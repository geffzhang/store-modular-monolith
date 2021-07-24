using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainExceptions
{
    public class InvalidEmailException : DomainException
    {
        public string Email { get; }
        public InvalidEmailException(string email) : base($"Invalid email: {email}.")
        {
            Email = email;
        }
    }
}