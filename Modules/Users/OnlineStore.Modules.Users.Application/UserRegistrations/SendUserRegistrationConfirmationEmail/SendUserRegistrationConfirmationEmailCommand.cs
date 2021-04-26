using System;
using Common.Messaging.Commands;
using Newtonsoft.Json;
using OnlineStore.Modules.Users.Domain.UserRegistrations;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.SendUserRegistrationConfirmationEmail
{
    public class SendUserRegistrationConfirmationEmailCommand : ICommand
    {
        [JsonConstructor]
        public SendUserRegistrationConfirmationEmailCommand(Guid id, UserRegistrationId userRegistrationId, string email)
        {
            Id = id;
            UserRegistrationId = userRegistrationId;
            Email = email;
        }

        internal UserRegistrationId UserRegistrationId { get; }
        internal string Email { get; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}