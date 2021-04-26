using System;
using System.Collections.Generic;
using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.DomainEvents;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.DomainServices;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.Rules;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Domain.UserRegistrations
{
    public class UserRegistration : AggregateRoot<Guid, UserRegistrationId>
    {
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Name { get; }
        public string Login { get; }
        public string Password { get; }
        public DateTime RegisterDate { get; }
        public DateTime? ConfirmedDate { get; private set; }
        public UserRegistrationStatus Status { get; private set; }
        public Address Address { get; }

        private UserRegistration()
        {
            // Only EF.
        }

        private UserRegistration(
            string login,
            string password,
            string email,
            string firstName,
            string lastName,
            Address address,
            IUsersCounter usersCounter,
            string confirmLink)
        {
            CheckRule(new UserLoginMustBeUniqueRule(usersCounter, login));

            Id = new UserRegistrationId(Guid.NewGuid());
            Login = login;
            Password = password;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            Name = $"{firstName} {lastName}";
            RegisterDate = DateTime.UtcNow;
            Status = UserRegistrationStatus.WaitingForConfirmation;

            AddDomainEvent(new NewUserRegisteredDomainEvent(Id, Login, Email, FirstName, LastName, Name, RegisterDate,
                Address, confirmLink));
        }

        public static UserRegistration RegisterNewUser(
            string login,
            string password,
            string email,
            string firstName,
            string lastName,
            Address address,
            IUsersCounter usersCounter,
            string confirmLink)
        {
            return new(login, password, email, firstName, lastName, address, usersCounter, confirmLink);
        }
        
        public User CreateUser()
        {
            this.CheckRule(new UserCannotBeCreatedWhenRegistrationIsNotConfirmedRule(Status));
            return null;
            // return User.Of(new UserId(Id.Id), Email, FirstName, LastName, Name, Login, Password, Created,new List<Permission>(){P}
        }

        public void Confirm()
        {
            CheckRule(new UserRegistrationCannotBeConfirmedMoreThanOnceRule(Status));
            CheckRule(new UserRegistrationCannotBeConfirmedAfterExpirationRule(Status));

            Status = UserRegistrationStatus.Confirmed;
            ConfirmedDate = DateTime.UtcNow;

            AddDomainEvent(new UserRegistrationConfirmedDomainEvent(Id));
        }

        public void Expire()
        {
            CheckRule(new UserRegistrationCannotBeExpiredMoreThanOnceRule(Status));

            Status = UserRegistrationStatus.Expired;

            AddDomainEvent(new UserRegistrationExpiredDomainEvent(Id));
        }
    }
}