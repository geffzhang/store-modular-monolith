using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Exceptions
{
    internal class InvalidRefreshTokenException : DomainException
    {
        public InvalidRefreshTokenException() : base("Invalid refresh token.")
        {
        }
    }
}