using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Exceptions
{
    internal class InvalidCredentialsException : DomainException
    {
        public InvalidCredentialsException(string name) : base("Invalid credentials.")
        {
            Name = name;
        }

        public string Name { get; }
    }
}