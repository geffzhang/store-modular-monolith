using System;
using Common.Core.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.Features.Users.ChangeUserPassword
{
    public class ChangeUserPasswordCommand : ICommand
    {
        public ChangeUserPasswordCommand(string userName, string oldPassword, string newPassword)
        {
            UserName = userName;
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }

        public string UserName { get; }
        public string OldPassword { get; }
        public string NewPassword { get; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}