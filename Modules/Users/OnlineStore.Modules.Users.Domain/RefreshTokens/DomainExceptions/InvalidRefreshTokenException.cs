using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.RefreshTokens.DomainExceptions
{
    internal class InvalidRefreshTokenException : DomainException
    {
        public InvalidRefreshTokenException() : base("Invalid refresh token.")
        {
        }
    }
}