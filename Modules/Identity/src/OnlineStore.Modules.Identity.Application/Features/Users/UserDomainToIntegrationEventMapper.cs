using System.Collections.Generic;
using System.Linq;
using Common.Domain;
using Common.Messaging;
using Common.Messaging.Events;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser;
using OnlineStore.Modules.Identity.Domain.Users.DomainEvents;

namespace OnlineStore.Modules.Identity.Application.Features.Users
{
    public class UserDomainToIntegrationEventMapper : IUserDomainToIntegrationEventMapper
    {
        public IEnumerable<IIntegrationEvent?> Map(params IDomainEvent[] events) => events.Select(Map);

        private static IIntegrationEvent? Map(IDomainEvent @event)
            => @event switch
            {
                NewUserRegisteredDomainEvent e => new NewUserRegisteredIntegrationEvent(e.User.Id, e.User.UserName,
                    e.User.Email, e.User.FirstName, e.User.LastName, e.User.Name),
                _ => null
            };
    }
}