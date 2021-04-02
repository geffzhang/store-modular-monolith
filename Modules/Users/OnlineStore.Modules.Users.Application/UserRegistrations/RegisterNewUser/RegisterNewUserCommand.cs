using System;
using System.Collections.Generic;
using System.Linq;
using Common.Messaging.Commands;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.RegisterNewUser
{
    public class RegisterNewUserCommand : ICommand
    {
        public string Login { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CorrelationId { get; set; }
        public Guid UserId { get; }
        public string Name { get; }
        public string Password { get; }
        public IEnumerable<string> Permissions { get; }
        public IEnumerable<string> Roles { get; }

        public RegisterNewUserCommand(Guid userId, string login, string password, string email, string firstName,
            string lastName,
            string name, IEnumerable<string> roles = null,
            IEnumerable<string> permissions = null)
        {
            UserId = userId == Guid.Empty ? Guid.NewGuid() : userId;
            Login = login;
            Password = password;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Name = name;
            Roles = roles ?? Enumerable.Empty<string>();
            Permissions = permissions ?? Enumerable.Empty<string>();
        }
    }
}