using System;
using BuildingBlocks.Core.Domain.DomainEventNotifications;
using BuildingBlocks.Core.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainEvents;

namespace OnlineStore.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class NewUserRegisteredNotification : DomainNotificationEventBase<NewUserRegisteredDomainEvent>
    {
        public NewUserRegisteredNotification(NewUserRegisteredDomainEvent domainEvent, Guid id, Guid correlationId) : base(domainEvent, id,
            correlationId)
        {
        }
    }
}