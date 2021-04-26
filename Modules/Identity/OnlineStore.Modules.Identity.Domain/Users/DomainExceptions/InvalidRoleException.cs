using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Users.DomainExceptions
{
    internal class InvalidRoleException : DomainException
    {
        public InvalidRoleException(string role) : base($"Invalid role: {role}.")
        {
        }
    }
}