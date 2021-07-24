using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Exceptions
{
    internal class InvalidAggregateIdException : DomainException
    {
        public InvalidAggregateIdException() : base("Invalid aggregate id.")
        {
        }
    }
}