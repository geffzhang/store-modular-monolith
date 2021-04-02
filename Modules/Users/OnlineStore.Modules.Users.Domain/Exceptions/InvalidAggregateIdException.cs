using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Exceptions
{
    internal class InvalidAggregateIdException : DomainException
    {
        public InvalidAggregateIdException() : base("Invalid aggregate id.")
        {
        }
    }
}