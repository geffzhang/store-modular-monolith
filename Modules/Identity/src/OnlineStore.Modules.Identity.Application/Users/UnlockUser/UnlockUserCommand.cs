using System;
using Common.Core.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.Users.UnlockUser
{
    public class UnlockUserCommand : ICommand
    {
        public Guid Id { get; }
    }
}