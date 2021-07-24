using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainExceptions
{
    internal class InvalidPasswordException : DomainException
    {
        public InvalidPasswordException() : base("Invalid password.")
        {
        }
    }
}