using System.Collections.Generic;
using Common.Domain;
using Common.Messaging;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Contracts
{
    public interface IUserDomainToIntegrationEventMapper
    {
        IEnumerable<IMessage> Map(params IDomainEvent[] events);
    }
}