using System;
using Common.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.Features.Users.SendVerificationEmail
{
    public class SendVerificationEmailCommand : ICommand
    {
        public SendVerificationEmailCommand(string userId, string requestScheme, string requestHost)
        {
            UserId = userId;
            RequestScheme = requestScheme;
            RequestHost = requestHost;
        }

        public string UserId { get; }
        public string RequestScheme { get; }
        public string RequestHost { get; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}