using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Users.Domain.Users.DomainEvents
{
    internal class SignedIn : DomainEventBase<Guid>
    {
        public SignedIn(Guid userId)
        {
            UserId = userId;
        }

        public Guid CorrelationId { get; set; }
        public Guid UserId { get; }
    }
}