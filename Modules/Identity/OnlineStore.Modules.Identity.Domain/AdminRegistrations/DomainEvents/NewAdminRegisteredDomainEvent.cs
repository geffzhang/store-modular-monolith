using System;
using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.UserRegistrations;

namespace OnlineStore.Modules.Identity.Domain.AdminRegistrations.DomainEvents
{
    public class NewAdminRegisteredDomainEvent : DomainEventBase
    {
        public NewAdminRegisteredDomainEvent(
            AdminRegistrationId adminRegistrationId,
            string login,
            string email,
            string firstName,
            string lastName,
            string name,
            DateTime registerDate)
        {
            AdminRegistrationId = adminRegistrationId;
            Login = login;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Name = name;
            RegisterDate = registerDate;
        }

        public AdminRegistrationId AdminRegistrationId { get; }

        public string Login { get; }

        public string Email { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Name { get; }
        
        public DateTime RegisterDate { get; }
    }
}