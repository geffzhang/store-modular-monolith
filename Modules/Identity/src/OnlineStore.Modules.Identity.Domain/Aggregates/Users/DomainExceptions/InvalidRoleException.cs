using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainExceptions
{
    internal class InvalidRoleException : DomainException
    {
        public string Role { get; }

        public InvalidRoleException(string role) : base($"Invalid role: {role}.")
        {
            Role = role;
        }
    }
}