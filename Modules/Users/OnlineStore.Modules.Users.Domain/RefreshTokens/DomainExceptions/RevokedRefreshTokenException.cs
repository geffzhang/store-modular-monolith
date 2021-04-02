using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.RefreshTokens.DomainExceptions
{
    internal class RevokedRefreshTokenException : DomainException
    {
        public RevokedRefreshTokenException() : base("Revoked refresh token.")
        {
        }
    }
}