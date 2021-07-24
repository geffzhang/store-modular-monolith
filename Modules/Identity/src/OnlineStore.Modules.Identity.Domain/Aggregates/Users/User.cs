using System;
using System.Collections.Generic;
using Common.Core.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainEvents;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainExceptions;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Types;
using OnlineStore.Modules.Identity.Domain.Constants;

namespace OnlineStore.Modules.Identity.Domain.Aggregates.Users
{
    public class User : AggregateRoot<Guid, UserId>
    {
        // Using a private collection field, better for DDD Aggregate's encapsulation
        private readonly List<Role> _roles = new();

        // private readonly List<ApplicationUserLogin> _logins = new();
        private readonly List<Permission> _permissions = new();
        public string UserName { get; private set; }
        public bool EmailConfirmed { get; private set; }
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Name { get; private set; }
        public bool IsAdministrator { get; private set; }
        public string? PhotoUrl { get; private set; }
        public UserType UserType { get; private set; }
        public string? Status { get; private set; }
        public string Password { get; private set; }
        public bool LockoutEnabled { get; private set; }
        public bool IsActive { get; private set; }
        public bool PasswordExpired { get; private set; }
        public DateTime? LastPasswordChangedDate { get; private set; }
        public DateTime CreatedDate { get; init; }
        public DateTime? ModifiedDate { get; init; }
        public string? CreatedBy { get; init; }
        public string? ModifiedBy { get; init; }
        public IReadOnlyList<Role> Roles => _roles;
        public IReadOnlyList<Permission> Permissions => _permissions;

        private User(UserId id,
            string email,
            string firstName,
            string lastName,
            string name,
            string userName,
            string password,
            UserType userType,
            bool isAdmin = false,
            bool isActive = true,
            bool locked = false,
            bool emailConfirmed = false,
            string? photoUrl = null,
            string? status = null,
            string? createdBy = null,
            DateTime? createdDate = null,
            string? modifiedBy = null,
            DateTime? modifiedDate = null)
        {
            CheckEmailValidity(email);
            CheckNameValidity(name);

            UserName = userName;
            IsActive = true;
            Id = id;
            Email = email.ToLowerInvariant();
            FirstName = firstName;
            LastName = lastName;
            Name = name.Trim();
            UserType = userType;
            Password = password;
            LockoutEnabled = locked;
            EmailConfirmed = emailConfirmed;
            PhotoUrl = photoUrl;
            Status = status;
            IsAdministrator = isAdmin;
            IsActive = isActive;
            CreatedDate = createdDate ?? DateTime.Now;
            CreatedBy = createdBy;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
        }

        private User()
        {
            // Only for deserialization 
        }

        public static User Of(UserId id,
            string email,
            string firstName,
            string lastName,
            string name,
            string userName,
            string password,
            UserType userType,
            bool isAdmin = false,
            bool isActive = true,
            bool locked = false,
            bool emailConfirmed = false,
            string? photoUrl = null,
            string? status = null,
            string? createdBy = null,
            DateTime? createdDate = null,
            string? modifiedBy = null,
            DateTime? modifiedDate = null)
        {
            var user = new User(id,
                email,
                firstName,
                lastName,
                name,
                userName,
                password,
                userType,
                isAdmin,
                isActive,
                locked,
                emailConfirmed,
                photoUrl,
                status,
                createdBy,
                createdDate,
                modifiedBy,
                modifiedDate);

            user.AddDomainEvent(new NewUserRegisteredDomainEvent(user));
            
            return user;
        }

        public void AssignRole(params Role[]? roles)
        {
            if (roles is null)
                throw new Exception("Role can't be null.");

            foreach (var role in roles)
            {
                var exists = _roles.Contains(role);
                if (!exists) _roles.Add(role);
            }
        }

        public void AssignPermission(params Permission[]? permissions)
        {
            if (permissions is null)
                throw new Exception("Role can't be null.");

            foreach (var permission in permissions)
            {
                var exists = _permissions.Contains(permission);
                if (!exists) _permissions.Add(permission);
            }
        }

        public virtual void Patch(User target)
        {
        }

        public static void CheckEmailValidity(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidEmailException(email);

            if (!RegexConstants.Email.IsMatch(email))
                throw new InvalidEmailException(email);
        }

        #region Domain Operations

        #endregion

        #region Domain Invariants

        public static void CheckNameValidity(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidNameException(name);
        }

        #endregion
    }
}