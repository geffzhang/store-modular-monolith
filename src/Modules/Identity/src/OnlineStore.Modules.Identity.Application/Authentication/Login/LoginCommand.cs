using System;
using BuildingBlocks.Core.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.Authentication.Login
{
    public class LoginCommand : ICommand
    {
        public LoginCommand(string userNameOrEmail, string password, bool remember)
        {
            UserNameOrEmail = userNameOrEmail;
            Password = password;
            Remember = remember;
        }

        public string UserNameOrEmail { get; }
        public string Password { get; }
        public bool Remember { get; }
        public Guid Id { get; } = Guid.NewGuid();
    }
}