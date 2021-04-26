using System;
using Common.Domain.Types;
using Newtonsoft.Json;
using OnlineStore.Modules.Users.Domain.UserRegistrations.DomainEvents;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.RegisterNewUser
{
    public class NewUserRegisteredNotification : DomainNotificationEventBase<NewUserRegisteredDomainEvent>
    {
        [JsonConstructor]
        public NewUserRegisteredNotification(NewUserRegisteredDomainEvent domainEvent, Guid id) : base(domainEvent, id)
        {
        }
    }
}