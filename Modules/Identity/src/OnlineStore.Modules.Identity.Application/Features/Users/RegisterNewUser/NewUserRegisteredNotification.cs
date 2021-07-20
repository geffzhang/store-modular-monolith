using System;
using Common.Core.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users.DomainEvents;

namespace OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser
{
    public class NewUserRegisteredNotification : DomainNotificationEventBase<NewUserRegisteredDomainEvent>
    {
        public NewUserRegisteredNotification(NewUserRegisteredDomainEvent domainEvent, Guid id, Guid correlationId) : base(domainEvent, id,
            correlationId)
        {
        }
    }
}