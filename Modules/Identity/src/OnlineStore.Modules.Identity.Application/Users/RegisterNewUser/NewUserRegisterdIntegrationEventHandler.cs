using System.Threading.Tasks;
using Common.Core.Messaging.Events;

namespace OnlineStore.Modules.Identity.Application.Users.RegisterNewUser
{
    public class NewUserRegisteredIntegrationEventHandler : IIntegrationEventHandler<NewUserRegisteredIntegrationEvent>
    {
        public Task HandleAsync(NewUserRegisteredIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}