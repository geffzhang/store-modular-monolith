using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Messaging.Events;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.RegisterNewUser
{
    public class NewUserRegisteredPublishEventHandler : IEventHandler<NewUserRegisteredNotification>
    {
        private readonly ICommandProcessor _commandProcessor;

        public NewUserRegisteredPublishEventHandler(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        public Task Handle(NewUserRegisteredNotification notification, CancellationToken cancellationToken)
        {
            _commandProcessor.PublishMessageAsync(new NewUserRegisteredIntegrationEvent(
                notification.Id,
                notification.DomainEvent.OccurredOn,
                notification.DomainEvent.UserRegistrationId.Value,
                notification.DomainEvent.Login,
                notification.DomainEvent.Email,
                notification.DomainEvent.FirstName,
                notification.DomainEvent.LastName,
                notification.DomainEvent.Name));

            return Task.CompletedTask;
        }

        public Task HandleAsync(NewUserRegisteredNotification @event)
        {
            throw new System.NotImplementedException();
        }
    }
}