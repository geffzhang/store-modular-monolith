using System.Collections.Generic;
using Common.Core.Domain;

namespace OnlineStore.Modules.Identity.Application.Users.Contracts
{
    public interface IUserDomainToIntegrationEventMapper
    {
        IEnumerable<dynamic?> Map(params IDomainEvent[] events);
    }
}