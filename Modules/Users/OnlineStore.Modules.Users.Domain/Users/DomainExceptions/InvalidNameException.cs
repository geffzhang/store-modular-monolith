using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Exceptions
{
    internal class InvalidNameException : DomainException
    {
        public InvalidNameException(string name) : base($"Invalid name: {name}.")
        {
        }
    }
}