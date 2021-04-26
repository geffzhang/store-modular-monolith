using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainExceptions
{
    internal class InvalidNameException : DomainException
    {
        public InvalidNameException(string name) : base($"Invalid name: {name}.")
        {
        }
    }
}