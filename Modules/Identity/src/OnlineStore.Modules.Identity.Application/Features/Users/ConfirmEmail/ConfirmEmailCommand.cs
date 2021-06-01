using System;
using Common.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.Features.Users.ConfirmEmail
{
    public class ConfirmEmailCommand : ICommand
    {
        public ConfirmEmailCommand(string userId, string code)
        {
            UserId = userId;
            Code = code;
        }

        public string UserId { get; }
        public string Code { get; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}