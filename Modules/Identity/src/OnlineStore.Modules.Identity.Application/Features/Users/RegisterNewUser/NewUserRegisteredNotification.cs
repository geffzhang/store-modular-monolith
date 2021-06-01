using System;
using Common.Domain.Types;
using Newtonsoft.Json;
using OnlineStore.Modules.Identity.Domain.Users.DomainEvents;

namespace OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser
{
    public class NewUserRegisteredNotification : DomainNotificationEventBase<NewUserRegisteredDomainEvent>
    {
    }
}