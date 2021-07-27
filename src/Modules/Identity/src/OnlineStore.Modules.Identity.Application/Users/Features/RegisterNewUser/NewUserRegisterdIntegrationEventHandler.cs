using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Domain.IntegrationEvents;
using BuildingBlocks.Core.Messaging;

namespace OnlineStore.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class NewUserRegisteredIntegrationEventHandler : IIntegrationEventHandler<NewUserRegisteredIntegrationEvent>
    {
        public Task HandleAsync(NewUserRegisteredIntegrationEvent @event, IMessageContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}