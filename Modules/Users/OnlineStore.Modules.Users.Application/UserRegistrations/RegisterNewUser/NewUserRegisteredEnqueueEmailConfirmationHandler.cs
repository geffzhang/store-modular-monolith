using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Messaging.Events;
using Common.Messaging.Scheduling;
using Common.Scheduling;
using OnlineStore.Modules.Users.Application.UserRegistrations.SendUserRegistrationConfirmationEmail;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.RegisterNewUser
{
    public class NewUserRegisteredEnqueueEmailConfirmationHandler : IEventHandler<NewUserRegisteredNotification>
    {
        private readonly IMessagesScheduler _messagesScheduler;

        public NewUserRegisteredEnqueueEmailConfirmationHandler(IMessagesScheduler messagesScheduler)
        {
            _messagesScheduler = messagesScheduler;
        }

        public async Task HandleAsync(NewUserRegisteredNotification @event)
        {
            await _messagesScheduler.EnqueueAsync(new SendUserRegistrationConfirmationEmailCommand(
                Guid.NewGuid(),
                @event.DomainEvent.UserRegistrationId,
                @event.DomainEvent.Email));
        }
    }
}