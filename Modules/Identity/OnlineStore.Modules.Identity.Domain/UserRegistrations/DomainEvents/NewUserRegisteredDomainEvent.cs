using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.UserRegistrations.DomainEvents
{
    public class NewUserRegisteredDomainEvent : DomainEventBase
    {
        public NewUserRegisteredDomainEvent(
            UserRegistrationId userRegistrationId,
            string login,
            string email,
            string firstName,
            string lastName,
            string name,
            DateTime registerDate,
            Address address,
            string confirmLink)
        {
            UserRegistrationId = userRegistrationId;
            Login = login;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Name = name;
            RegisterDate = registerDate;
            Address = address;
            ConfirmLink = confirmLink;
        }

        public UserRegistrationId UserRegistrationId { get; }

        public string Login { get; }

        public string Email { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Name { get; }
        
        public string ConfirmLink { get; }
        
        public Address Address { get; }
        
        public DateTime RegisterDate { get; }
    }
}