using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Core.Domain;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.RegisterNewUser;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainEvents;

namespace OnlineStore.Modules.Identity.Application.Users
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