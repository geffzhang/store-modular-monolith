using System.Collections.Generic;
using System.Linq;
using Common.Core.Domain;
using Common.Core.Messaging.Events;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser;
using OnlineStore.Modules.Identity.Domain.Users.DomainEvents;

namespace OnlineStore.Modules.Identity.Application.Features.Users
{
    public class UserDomainToIntegrationEventMapper : IUserDomainToIntegrationEventMapper
    {
        public IEnumerable<dynamic?> Map(params IDomainEvent[] events) => events.Select(Map);

        private static dynamic? Map(IDomainEvent @event)
            => @event switch
            {
                NewUserRegisteredDomainEvent e => new NewUserRegisteredIntegrationEvent(e.User.Id, e.User.UserName,
                    e.User.Email, e.User.FirstName, e.User.LastName, e.User.Name),
                _ => null
            };
    }
}