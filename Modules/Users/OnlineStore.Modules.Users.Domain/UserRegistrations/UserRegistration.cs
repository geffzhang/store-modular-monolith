using System;
using System.Collections.Generic;
using Common.Domain.Types;
using OnlineStore.Modules.Users.Domain.UserRegistrations.DomainEvents;
using OnlineStore.Modules.Users.Domain.UserRegistrations.DomainServices;
using OnlineStore.Modules.Users.Domain.UserRegistrations.Rules;
using OnlineStore.Modules.Users.Domain.Users;

namespace OnlineStore.Modules.Users.Domain.UserRegistrations
{
    public class UserRegistration : AggregateRoot<Guid, UserRegistrationId>
    {
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Name { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }
        public DateTime RegisterDate { get; private set; }
        public DateTime? ConfirmedDate { get; private set; }
        public UserRegistrationStatus Status { get; private set; }

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
            IUsersCounter usersCounter)
        {
            CheckRule(new UserLoginMustBeUniqueRule(usersCounter, login));

            Id = new UserRegistrationId(Guid.NewGuid());
            Login = login;
            Password = password;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Name = $"{firstName} {lastName}";
            RegisterDate = DateTime.UtcNow;
            Status = UserRegistrationStatus.WaitingForConfirmation;

            AddDomainEvent(new NewUserRegisteredDomainEvent(Id, Login, Email, FirstName, LastName, Name, RegisterDate));
        }

        public static UserRegistration RegisterNewUser(
            string login,
            string password,
            string email,
            string firstName,
            string lastName,
            IUsersCounter usersCounter)
        {
            return new(login, password, email, firstName, lastName, usersCounter);
        }

        public User CreateUser(IReadOnlyList<string> permissions, IReadOnlyList<string> roles)
        {
            CheckRule(new UserCannotBeCreatedWhenRegistrationIsNotConfirmedRule(Status));

            return User.Of(
                Id,
                Email,
                FirstName,
                LastName,
                Name,
                Login,
                Password,
                DateTime.Now,
                permissions,
                roles,
                false);
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