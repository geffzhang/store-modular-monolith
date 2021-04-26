using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainExceptions
{
    internal class InvalidRoleException : DomainException
    {
        public InvalidRoleException(string role) : base($"Invalid role: {role}.")
        {
        }
    }
}