using System;
using Common.Messaging.Events;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.RegisterNewUser
{
    public class NewUserRegisteredIntegrationEvent : IntegrationEventBase
    {
        public NewUserRegisteredIntegrationEvent(Guid id) : base(id)
        {
        }
    }
}