using System;
using Common.Core.Messaging.Events;

namespace OnlineStore.Modules.Identity.Application.Users.RegisterNewUser
{
    public class NewUserRegisteredIntegrationEvent : IntegrationEventBase
    {
        public Guid UserId { get; }

        public string Login { get; }

        public string Email { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Name { get; }

        public NewUserRegisteredIntegrationEvent(Guid userId, string login, string email, string firstName,
            string lastName, string name)
        {
            UserId = userId;
            Login = login;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Name = name;
        }
    }
}