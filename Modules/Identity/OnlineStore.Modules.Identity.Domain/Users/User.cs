using System;
using System.Collections.Generic;
using System.Linq;
using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Users.DomainEvents;
using OnlineStore.Modules.Identity.Domain.Users.DomainExceptions;

namespace OnlineStore.Modules.Identity.Domain.Users
{
    public class User : AggregateRoot<Guid, UserId>
    {
        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method UserAggregateRoot.AddAddress() which includes behaviour.
        private readonly List<Role> _userRoles;

        // private readonly List<ApplicationUserLogin> _logins;
        private readonly List<Permission> _permissions;
        public string UserName { get; }
        public bool EmailConfirmed { get; }
        public string Email { get; }

        /// <summary>
        /// Tenant id
        /// </summary>
        public string StoreId { get; }

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
        public IReadOnlyList<Role> Roles { get; }

        /// <summary>
        /// External provider logins.
        /// </summary>
        // public IReadOnlyList<ApplicationUserLogin> Logins { get; }

        /// <summary>
        /// All user permissions
        /// </summary>
        public IEnumerable<Permission> Permissions { get; }

        /// <summary>
        /// Indicates that the password for this user is expired and must be changed.
        /// </summary>
        public bool PasswordExpired { get; }

        /// <summary>
        /// The last date when the password was changed
        /// </summary>
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
                if (!Role.IsValid(role)) throw new InvalidRoleException(role);
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
            Roles = roles?.Select(x => Role.Of(x)).ToList();
            Permissions = permissions.Select(x => Permission.Of(x, "")).ToList();
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


        // public void AssignAddress(Address address)
        // {
        //     if (address is  null)
        //         throw new Exception("Address can't be null.");
        //
        //     var exists = this._userAddresses.Contains(address);
        //
        //     if (!exists) this._userAddresses.Add(address);
        // }

        public void AssignRole(Role role)
        {
            if (role is null)
                throw new Exception("Role can't be null.");

            var exists = _userRoles.Contains(role);

            if (!exists) _userRoles.Add(role);
        }

        public static User Of(UserId id, string email, string firstName, string lastName,
            string name, string userName, string password, DateTime createdDate, string createdBy,
            IReadOnlyList<string> permissions, string userType, bool isAdmin = false, bool isActive = true,
            IReadOnlyList<string> roles = null, bool locked = false, bool emailConfirmed = false,
            string photoUrl = null, string status = null, string modifiedBy = null,
            DateTime? modifiedDate = null)
        {
            return new User(id, email, firstName, lastName, name, userName, password, createdDate, createdBy,
                permissions, userType,
                isAdmin, isActive, roles, locked, emailConfirmed, photoUrl, status, modifiedBy, modifiedDate);
        }
    }
}