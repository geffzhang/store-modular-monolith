using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Domain;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;

namespace OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser
{
    public class NewUserRegisteredPublishEventHandler : IDomainNotificationEventHandler<NewUserRegisteredNotification>
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserDomainToIntegrationEventMapper _userDomainToIntegrationEventMapper;

        public NewUserRegisteredPublishEventHandler(ICommandProcessor commandProcessor,
            IUserDomainToIntegrationEventMapper userDomainToIntegrationEventMapper)
        {
            _commandProcessor = commandProcessor;
            _userDomainToIntegrationEventMapper = userDomainToIntegrationEventMapper;
        }

        public async Task HandleAsync(NewUserRegisteredNotification notification)
        {
            var integrationEvents = _userDomainToIntegrationEventMapper.Map(notification.DomainEvent).ToArray();
            await _commandProcessor.PublishMessageAsync(integrationEvents);

            // await _commandProcessor.PublishMessageAsync(new NewUserRegisteredIntegrationEvent(
            //     notification.DomainEvent.User.Id.Id,
            //     notification.DomainEvent.User.UserName,
            //     notification.DomainEvent.User.Email,
            //     notification.DomainEvent.User.FirstName,
            //     notification.DomainEvent.User.LastName,
            //     notification.DomainEvent.User.Name));
        }
    }
}