using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Exceptions
{
    internal class InvalidPasswordException : DomainException
    {
        public InvalidPasswordException() : base("Invalid password.")
        {
        }
    }
}