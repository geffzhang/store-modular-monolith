using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using CompanyName.MyMeetings.Modules.UserAccess.Application.UserRegistrations.SendUserRegistrationConfirmationEmail;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.RegisterNewUser
{
    public class NewUserRegisteredEnqueueEmailConfirmationHandler : IDomainNotificationEventHandler<NewUserRegisteredNotification>
    {
        private readonly ICommandsScheduler _commandsScheduler;

        public NewUserRegisteredEnqueueEmailConfirmationHandler(ICommandsScheduler commandsScheduler)
        {
            _commandsScheduler = commandsScheduler;
        }

        public async Task Handle(NewUserRegisteredNotification notification, CancellationToken cancellationToken)
        {
            await _commandsScheduler.EnqueueAsync(new SendUserRegistrationConfirmationEmailCommand(
                Guid.NewGuid(),
                notification.DomainEvent.UserRegistrationId,
                notification.DomainEvent.Email));
        }
    }
}