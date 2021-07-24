using System;
using BuildingBlocks.Core.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.Users.ValidatePasswordResetToken
{
    public class ValidatePasswordResetTokenCommand: ICommand
    {
        public ValidatePasswordResetTokenCommand(string userId,string token)
        {
            UserId = userId;
            Token = token;
        }

        public string UserId { get; }
        public string Token { get; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}