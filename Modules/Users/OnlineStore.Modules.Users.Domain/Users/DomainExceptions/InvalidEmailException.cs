using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Exceptions
{
    internal class InvalidEmailException : DomainException
    {
        public InvalidEmailException(string email) : base($"Invalid email: {email}.")
        {
        }
    }
}