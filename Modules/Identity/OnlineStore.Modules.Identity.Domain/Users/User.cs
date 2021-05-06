using System;
using System.Collections.Generic;
using System.Linq;
using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Users.DomainEvents;
using OnlineStore.Modules.Identity.Domain.Users.DomainExceptions;
using OnlineStore.Modules.Identity.Domain.Users.Types;

namespace OnlineStore.Modules.Identity.Domain.Users
{
    public class User : AggregateRoot<Guid, UserId>
    {
        // Using a private collection field, better for DDD Aggregate's encapsulation
        private readonly List<Role.Role> _roles;

        // private readonly List<ApplicationUserLogin> _logins;
        private readonly List<Permission> _permissions;
        public string UserName { get; }
        public bool EmailConfirmed { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Name { get; }
        public string MemberId { get; }
        public bool IsAdministrator { get; }
        public string PhotoUrl { get; }
        public string UserType { get; }
        public string Status { get; }
        public string Password { get; }
        public DateTime CreatedDate { get; }
        public DateTime? ModifiedDate { get; }
        public string CreatedBy { get; }
        public string ModifiedBy { get; }
        public bool LockoutEnabled { get; }
        public bool IsActive { get; }
        public IReadOnlyList<Role.Role> Roles => _roles;

        // public IReadOnlyList<ApplicationUserLogin> Logins { get; }

        public IReadOnlyList<Permission> Permissions => _permissions;
        public bool PasswordExpired { get; }
        public DateTime? LastPasswordChangedDate { get; }

        private User()
        {
            // Only for EF.
        }

        private User(UserId id, string email, string firstName, string lastName,
            string name, string userName, string password, DateTime createdDate, string createdBy,
            IReadOnlyList<string> permissions, string userType, bool isAdmin = false, bool isActive = true,
            IReadOnlyList<string> roles = null, bool locked = false, bool emailConfirmed = false,
            string photoUrl = null, string status = null, string modifiedBy = null,
            DateTime? modifiedDate = null)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new InvalidEmailException(email);

            if (string.IsNullOrWhiteSpace(name)) throw new InvalidNameException(name);

            if (string.IsNullOrWhiteSpace(password)) throw new InvalidPasswordException();

            roles?.ToList().ForEach(role =>
            {
                if (!Role.Role.IsValid(role)) throw new InvalidRoleException(role);
            });

            UserName = userName;
            IsActive = true;
            Id = id;
            Email = email.ToLowerInvariant();
            FirstName = firstName;
            LastName = lastName;
            Name = name.Trim();
            UserType = userType;
            Password = password;
            CreatedDate = createdDate;
            LockoutEnabled = locked;
            _roles = roles?.Select(x => Role.Role.Of(x)).ToList();
            _permissions = permissions?.Select(x => Permission.Of(x, "")).ToList() ?? new List<Permission>();
            EmailConfirmed = emailConfirmed;
            PhotoUrl = photoUrl;
            Status = status;
            CreatedBy = createdBy;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            IsAdministrator = isAdmin;
            IsActive = isActive;

            AddDomainEvent(new UserCreatedDomainEvent(Id));
        }

        public static User Of(UserId id, string email, string firstName, string lastName,
            string name, string userName, string password, DateTime createdDate, string createdBy,
            IReadOnlyList<string> permissions, string userType, bool isAdmin = false, bool isActive = true,
            IReadOnlyList<string> roles = null, bool locked = false, bool emailConfirmed = false,
            string photoUrl = null, string status = null, string modifiedBy = null,
            DateTime? modifiedDate = null)
        {
            return new(id, email, firstName, lastName, name, userName, password, createdDate, createdBy,
                permissions, userType,
                isAdmin, isActive, roles, locked, emailConfirmed, photoUrl, status, modifiedBy, modifiedDate);
        }

        public void AssignRole(Role.Role role)
        {
            if (role is null)
                throw new Exception("Role can't be null.");

            var exists = _roles.Contains(role);

            if (!exists) _roles.Add(role);
        }

        public virtual void Patch(User target)
        {
        }
    }
}