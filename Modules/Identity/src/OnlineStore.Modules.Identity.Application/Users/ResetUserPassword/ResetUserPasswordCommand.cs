using System;
using Common.Core.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.Users.ResetUserPassword
{
    public class ResetUserPasswordCommand : ICommand
    {
        public ResetUserPasswordCommand(string userName, string userId, string token, string newPassword,
            bool forcePasswordChangeOnNextSignIn)
        {
            UserName = userName;
            UserId = userId;
            Token = token;
            NewPassword = newPassword;
            ForcePasswordChangeOnNextSignIn = forcePasswordChangeOnNextSignIn;
        }

        public string UserName { get; }
        public string UserId { get; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public bool ForcePasswordChangeOnNextSignIn { get; set; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}