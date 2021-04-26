using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainExceptions
{
    internal class InvalidEmailException : DomainException
    {
        public InvalidEmailException(string email) : base($"Invalid email: {email}.")
        {
        }
    }
}