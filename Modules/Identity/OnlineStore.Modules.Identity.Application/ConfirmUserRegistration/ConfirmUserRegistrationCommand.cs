using System;
using Common.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.ConfirmUserRegistration
{
    public class ConfirmUserRegistrationCommand : ICommand
    {
        public ConfirmUserRegistrationCommand(Guid userRegistrationId)
        {
            UserRegistrationId = userRegistrationId;
        }

        public Guid UserRegistrationId { get; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CorrelationId { get; set; }
    }
}