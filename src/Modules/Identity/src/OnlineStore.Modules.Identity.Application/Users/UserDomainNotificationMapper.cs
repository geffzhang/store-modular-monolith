using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Core.Domain.DomainEventNotifications;
using OnlineStore.Modules.Identity.Application.Users.Features.RegisterNewUser;

namespace OnlineStore.Modules.Identity.Application.Users
{
    public class UserDomainNotificationMapper : IDomainNotificationsMapper
    {
        readonly Dictionary<string, Type> _mapping = new();

        public UserDomainNotificationMapper()
        {
            _mapping.Add("NewUserRegisteredNotification", typeof(NewUserRegisteredNotification));
        }

        public string? GetName(Type type)
        {
            return _mapping.ToDictionary((i) => i.Value, (i) => i.Key).GetValueOrDefault(type);
        }

        public Type? GetType(string name)
        {
            return _mapping.GetValueOrDefault(name);
        }
    }
}