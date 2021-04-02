using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Exceptions
{
    internal class RevokedRefreshTokenException : DomainException
    {
        public RevokedRefreshTokenException() : base("Revoked refresh token.")
        {
        }
    }
}