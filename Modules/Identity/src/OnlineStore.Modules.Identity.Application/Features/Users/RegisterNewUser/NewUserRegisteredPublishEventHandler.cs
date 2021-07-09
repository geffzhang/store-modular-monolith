using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Domain;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;

namespace OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser
{
    //http://www.kamilgrzybek.com/design/the-outbox-pattern/
    //http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/
    //http://www.kamilgrzybek.com/design/handling-domain-events-missing-part/
    public class NewUserRegisteredPublishEventHandler : IDomainEventNotificationHandler<NewUserRegisteredNotification>
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
        }
    }
}