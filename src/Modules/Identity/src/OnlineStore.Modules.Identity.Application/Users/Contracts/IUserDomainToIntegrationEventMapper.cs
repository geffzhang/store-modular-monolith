using System.Collections.Generic;
using BuildingBlocks.Core.Domain;

namespace OnlineStore.Modules.Identity.Application.Users.Contracts
{
    public interface IUserDomainToIntegrationEventMapper
    {
        IEnumerable<dynamic?> Map(params IDomainEvent[] events);
    }
}