using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainExceptions
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