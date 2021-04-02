using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Users.DomainExceptions
{
    internal class InvalidEmailException : DomainException
    {
        public InvalidEmailException(string email) : base($"Invalid email: {email}.")
        {
        }
    }
}