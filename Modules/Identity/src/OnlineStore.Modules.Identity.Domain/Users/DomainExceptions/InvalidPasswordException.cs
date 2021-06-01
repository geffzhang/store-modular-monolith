using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainExceptions
{
    internal class InvalidPasswordException : DomainException
    {
        public InvalidPasswordException() : base("Invalid password.")
        {
        }
    }
}