using System;
using Common.Domain;
using Common.Domain.Types;
using static Common.Utils.DateTimeHelper;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainEvents
{
    internal class UserLocked : DomainEventBase
    {
        public UserLocked(UserId userId)
        {
            UserId = userId;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CorrelationId { get; set; }
        public UserId UserId { get; }
        public DateTime CreatedAt => NewDateTime();
    }
}