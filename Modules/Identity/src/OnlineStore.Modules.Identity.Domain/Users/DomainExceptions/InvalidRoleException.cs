using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainExceptions
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