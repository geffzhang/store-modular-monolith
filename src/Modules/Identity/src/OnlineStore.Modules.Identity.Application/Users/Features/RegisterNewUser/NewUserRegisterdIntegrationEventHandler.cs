using System.Threading.Tasks;
using BuildingBlocks.Core.Domain.IntegrationEvents;
using BuildingBlocks.Core.Messaging.Events;

namespace OnlineStore.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class NewUserRegisteredIntegrationEventHandler : IIntegrationEventHandler<NewUserRegisteredIntegrationEvent>
    {
        public Task HandleAsync(NewUserRegisteredIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}