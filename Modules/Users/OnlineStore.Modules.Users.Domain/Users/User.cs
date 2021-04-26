using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Common.Domain.Types;
using OnlineStore.Modules.Users.Domain.Exceptions;
using OnlineStore.Modules.Users.Domain.UserRegistrations;
using OnlineStore.Modules.Users.Domain.Users.DomainEvents;
using OnlineStore.Modules.Users.Domain.Users.DomainExceptions;

namespace OnlineStore.Modules.Users.Domain.Users
{
    //https://github.com/dotnet-architecture/eShopOnWeb/issues/39
    //https://github.com/jasontaylordev/NorthwindTraders/blob/master/Src/Domain/Entities/Employee.cs
    public class User : AggregateRoot<Guid, UserId>
    {
        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method UserAggregateRoot.AddAddress() which includes behaviour.
        private readonly List<UserRole> _userRoles;
        private readonly List<string> _permissions = new();
        private readonly List<UserAddress> _userAddresses = new();

        public string Email { get; }
        public string Name { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public bool IsActive { get; }
        public string Login { get; }
        public string RefreshToken { get; }
        public string Password { get; }
        public DateTime CreatedAt { get; }
        public bool Locked { get; private set; }

        public IReadOnlyCollection<string> Permissions
        {
            get => _permissions;
            private init => _permissions = value.ToList();
        }

        public IReadOnlyCollection<UserRole> Roles => _userRoles;
        public IReadOnlyCollection<UserAddress> UserAddresses => _userAddresses;

        private User(UserRegistrationId userRegistrationId, string email, string firstName, string lastName,
            string name, string login, string password, DateTime createdAt,
            IReadOnlyList<string> roles = null, IReadOnlyList<string> permissions = null, bool locked = false)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new InvalidEmailException(email);

            if (string.IsNullOrWhiteSpace(name)) throw new InvalidNameException(name);

            if (string.IsNullOrWhiteSpace(password)) throw new InvalidPasswordException();

            roles?.ToList().ForEach(role =>
            {
                if (!UserRole.IsValid(role)) throw new InvalidRoleException(role);
            });

            Login = login;
            IsActive = true;
            Id = new UserId(userRegistrationId.Value);
            Email = email.ToLowerInvariant();
            FirstName = firstName;
            LastName = lastName;
            Name = name.Trim();
            Password = password;
            _userRoles = roles?.Select(UserRole.Of).ToList();
            CreatedAt = createdAt;
            Permissions = permissions?.ToImmutableList() ?? Enumerable.Empty<string>().ToImmutableList();
            Locked = locked;

            AddDomainEvent(new UserCreatedDomainEvent(Id));
        }
 
        public static User Of(UserRegistrationId userRegistrationId, string email, string firstName, string lastName,
            string name, string login, string password, DateTime createdAt,
            IReadOnlyList<string> permissions = null, IReadOnlyList<string> roles = null, bool locked = false)
        {
            return new User(userRegistrationId, email, firstName, lastName, name, login, password, createdAt, roles,
                permissions, locked);
        }

        public bool Lock()
        {
            if (Locked) return false;

            Locked = true;
            return true;
        }

        public bool Unlock()
        {
            if (!Locked) return false;

            Locked = false;
            return true;
        }
    }
}