using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Exceptions
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