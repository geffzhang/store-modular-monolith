using System.Collections.Generic;
using Common.Domain;
using Common.Messaging;
using Common.Messaging.Events;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Contracts
{
    public interface IUserDomainToIntegrationEventMapper
    {
        IEnumerable<IIntegrationEvent?> Map(params IDomainEvent[] events);
    }
}