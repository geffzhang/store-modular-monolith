using System;
using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.AdminRegistrations.DomainEvents;
using OnlineStore.Modules.Identity.Domain.AdminRegistrations.Rules;
using OnlineStore.Modules.Identity.Domain.UserRegistrations;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.DomainServices;

namespace OnlineStore.Modules.Identity.Domain.AdminRegistrations
{
    public class AdminRegistration : AggregateRoot<Guid, AdminRegistrationId>
    {
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Name { get; }
        public string Login { get; }
        public string Password { get; }
        public DateTime RegisterDate { get; }

        private AdminRegistration()
        {
            // Only EF.
        }

        private AdminRegistration(
            string login,
            string password,
            string email,
            string firstName,
            string lastName,
            IUsersCounter usersCounter)
        {
            CheckRule(new AdminLoginMustBeUniqueRule(login, usersCounter));

            Id = new AdminRegistrationId(Guid.NewGuid());
            Login = login;
            Password = password;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Name = $"{firstName} {lastName}";
            RegisterDate = DateTime.UtcNow;

            AddDomainEvent(
                new NewAdminRegisteredDomainEvent(Id, Login, Email, FirstName, LastName, Name, RegisterDate));
        }

        public static AdminRegistration RegisterNewAdmin(
            string login,
            string password,
            string email,
            string firstName,
            string lastName,
            IUsersCounter usersCounter)
        {
            return new(login, password, email, firstName, lastName, usersCounter);
        }
    }
}