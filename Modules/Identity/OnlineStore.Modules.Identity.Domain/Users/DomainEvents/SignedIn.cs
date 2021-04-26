using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainEvents
{
    internal class SignedIn : DomainEventBase
    {
        public SignedIn(Guid userId)
        {
            UserId = userId;
        }

        public Guid CorrelationId { get; set; }
        public Guid UserId { get; }
    }
}