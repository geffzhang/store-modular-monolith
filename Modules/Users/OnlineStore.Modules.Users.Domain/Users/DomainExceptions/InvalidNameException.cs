using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Users.DomainExceptions
{
    internal class InvalidNameException : DomainException
    {
        public InvalidNameException(string name) : base($"Invalid name: {name}.")
        {
        }
    }
}