using System.Threading.Tasks;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.DomainEvents;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainEvents;

namespace OnlineStore.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class NewUserRegisteredDomainEventHandler:IDomainEventHandler<NewUserRegisteredDomainEvent>
    {
        public Task HandleAsync(NewUserRegisteredDomainEvent domainEvent)
        {
          return Task.CompletedTask;
        }
    }
}