using System.Threading.Tasks;
using Common.Core.Messaging;

namespace OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser
{
    public class NewUserRegisteredIntegrationEventHandler : IMessageHandler<NewUserRegisteredIntegrationEvent>
    {
        public Task HandleAsync(NewUserRegisteredIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}