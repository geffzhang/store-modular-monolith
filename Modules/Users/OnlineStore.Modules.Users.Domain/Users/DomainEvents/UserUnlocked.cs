using System;
using Common.Domain;
using static Common.Utils.DateTimeHelper;

namespace OnlineStore.Modules.Users.Domain.Users.DomainEvents
{
    internal class UserUnlocked : IDomainEvent
    {
        public UserUnlocked(UserId userId)
        {
            UserId = userId;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CorrelationId { get; set; }
        public UserId UserId { get; }
        public DateTime CreatedAt => NewDateTime();
    }
}