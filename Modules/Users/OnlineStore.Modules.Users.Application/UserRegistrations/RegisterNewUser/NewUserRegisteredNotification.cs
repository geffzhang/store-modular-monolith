using System;
using Newtonsoft.Json;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.RegisterNewUser
{
    public class NewUserRegisteredNotification : DomainNotificationBase<NewUserRegisteredDomainEvent>
    {
        [JsonConstructor]
        public NewUserRegisteredNotification(NewUserRegisteredDomainEvent domainEvent, Guid id)
            : base(domainEvent, id)
        {
        }
    }
}