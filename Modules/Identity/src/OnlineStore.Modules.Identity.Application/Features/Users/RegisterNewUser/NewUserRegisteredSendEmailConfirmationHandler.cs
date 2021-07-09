using System.Threading.Tasks;
using Common.Domain;
using Common.Scheduling;
using Microsoft.AspNetCore.Http;
using OnlineStore.Modules.Identity.Application.Features.Users.SendVerificationEmail;

namespace OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser
{
    //http://www.kamilgrzybek.com/design/the-outbox-pattern/
    //http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/
    //http://www.kamilgrzybek.com/design/handling-domain-events-missing-part/
    public class NewUserRegisteredSendEmailConfirmationHandler : IDomainEventNotificationHandler<NewUserRegisteredNotification>
    {
        private readonly IMessagesScheduler _messagesScheduler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NewUserRegisteredSendEmailConfirmationHandler(IMessagesScheduler messagesScheduler,
            IHttpContextAccessor httpContextAccessor)
        {
            _messagesScheduler = messagesScheduler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(NewUserRegisteredNotification @event)
        {
            await _messagesScheduler.Enqueue(new SendVerificationEmailCommand(@event.DomainEvent.User.Id.Id.ToString(),
                _httpContextAccessor.HttpContext?.Request.Scheme,
                _httpContextAccessor.HttpContext?.Request.Host.Value));
        }
    }
}