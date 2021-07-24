using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainExceptions
{
    internal class InvalidNameException : DomainException
    {
        public string Name { get; }

        public InvalidNameException(string name) : base($"Invalid name: {name}.")
        {
            Name = name;
        }
    }
}