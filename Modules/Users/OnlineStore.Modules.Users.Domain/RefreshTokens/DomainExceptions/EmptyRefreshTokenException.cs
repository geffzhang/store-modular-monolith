using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Exceptions
{
    internal class EmptyRefreshTokenException : DomainException
    {
        public EmptyRefreshTokenException() : base("Empty refresh token.")
        {
        }
    }
}