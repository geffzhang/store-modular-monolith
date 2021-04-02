using System;
using Common.Domain.Types;
using OnlineStore.Modules.Users.Domain.Exceptions;
using OnlineStore.Modules.Users.Domain.Users;

namespace OnlineStore.Modules.Users.Domain.RefreshToken
{
    internal class RefreshToken : AggregateRoot
    {
        public RefreshToken(AggregateId id, UserId userId, string token, DateTime createdAt,
            DateTime? revokedAt = null) : base(id)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new EmptyRefreshTokenException();

            UserId = userId;
            Token = token;
            CreatedAt = createdAt;
            RevokedAt = revokedAt;
        }

        public UserId UserId { get; }
        public string Token { get; }
        public DateTime CreatedAt { get; }
        public DateTime? RevokedAt { get; private set; }
        public bool Revoked => RevokedAt.HasValue;

        public void Revoke(DateTime revokedAt)
        {
            if (Revoked) throw new RevokedRefreshTokenException();

            RevokedAt = revokedAt;
        }
    }
}