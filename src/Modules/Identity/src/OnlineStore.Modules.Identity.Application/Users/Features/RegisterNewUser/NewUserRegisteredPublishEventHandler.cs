using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.DomainEventNotifications;
using OnlineStore.Modules.Identity.Application.Users.Contracts;

namespace OnlineStore.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    //http://www.kamilgrzybek.com/design/the-outbox-pattern/
    //http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/
    //http://www.kamilgrzybek.com/design/handling-domain-events-missing-part/
    public class NewUserRegisteredPublishEventHandler : IDomainEventNotificationHandler<NewUserRegisteredNotification>
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserDomainEventsToIntegrationEventsMapper _userDomainToIntegrationEventMapper;
        
        public NewUserRegisteredPublishEventHandler(ICommandProcessor commandProcessor,
            IUserDomainEventsToIntegrationEventsMapper userDomainToIntegrationEventMapper)
        {
            _commandProcessor = commandProcessor;
            _userDomainToIntegrationEventMapper = userDomainToIntegrationEventMapper;
        }

        public async Task HandleAsync(NewUserRegisteredNotification notification)
        {
            var integrationEvents = _userDomainToIntegrationEventMapper.Map(notification.DomainEvent).ToArray();
            foreach (var integrationEvent in integrationEvents)
            {
                await _commandProcessor.PublishMessageAsync(integrationEvent);
            }
        }
    }
}