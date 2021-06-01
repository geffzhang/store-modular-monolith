using System;
using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users.DomainEvents;

namespace OnlineStore.Modules.Identity.Application.Features.Users.SendVerificationEmail
{
    public class SendVerificationEmailNotification : DomainNotificationEventBase<VerificationEmailSentEvent>
    {
        public SendVerificationEmailNotification(VerificationEmailSentEvent domainEvent, Guid id) :
            base(domainEvent, id)
        {
        }
    }
}