using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.RefreshTokens.DomainExceptions
{
    internal class EmptyRefreshTokenException : DomainException
    {
        public EmptyRefreshTokenException() : base("Empty refresh token.")
        {
        }
    }
}