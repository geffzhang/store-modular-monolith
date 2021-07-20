using System.Collections.Generic;
using Common.Core.Domain;
using Common.Core.Messaging.Events;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Contracts
{
    public interface IUserDomainToIntegrationEventMapper
    {
        IEnumerable<dynamic?> Map(params IDomainEvent[] events);
    }
}