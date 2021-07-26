using System;
using BuildingBlocks.Authentication.Jwt;
using BuildingBlocks.Cqrs.Commands;

namespace OnlineStore.Modules.Identity.Application.Authentication.Features.Login
{
    public class LoginCommand : ICommand<JsonWebToken>
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