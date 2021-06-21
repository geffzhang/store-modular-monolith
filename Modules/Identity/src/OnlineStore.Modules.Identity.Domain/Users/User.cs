using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common.Domain.Types;
using Microsoft.AspNetCore.WebUtilities;
using OnlineStore.Modules.Identity.Domain.Constants;
using OnlineStore.Modules.Identity.Domain.Users.DomainEvents;
using OnlineStore.Modules.Identity.Domain.Users.DomainExceptions;
using OnlineStore.Modules.Identity.Domain.Users.Rules;
using OnlineStore.Modules.Identity.Domain.Users.Services;
using OnlineStore.Modules.Identity.Domain.Users.Types;

namespace OnlineStore.Modules.Identity.Domain.Users
{
    public class User : AggregateRoot<Guid, UserId>
    {
        // Using a private collection field, better for DDD Aggregate's encapsulation
        private readonly List<Role> _roles;

        private readonly IUserEditable _userEditable;

        // private readonly List<ApplicationUserLogin> _logins;
        private readonly List<Permission> _permissions;
        public string UserName { get; }
        public bool EmailConfirmed { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Name { get; }
        public bool IsAdministrator { get; }
        public string PhotoUrl { get; }
        public UserType UserType { get; }
        public string Status { get; }
        public string Password { get; }
        public bool LockoutEnabled { get; }
        public bool IsActive { get; }
        public bool PasswordExpired { get; }
        public DateTime? LastPasswordChangedDate { get; }
        public DateTime CreatedDate { get; init; }
        public DateTime? ModifiedDate { get; init; }
        public string CreatedBy { get; init; }
        public string ModifiedBy { get; init; }

        public IReadOnlyList<Role> Roles => _roles;

        public IReadOnlyList<Permission> Permissions => _permissions;

        private User()
        {
            // Only for EF.
        }

        private User(UserId id,
            string email,
            string firstName,
            string lastName,
            string name,
            string userName,
            string password,
            IReadOnlyList<string> permissions,
            UserType userType,
            IUserEditable userEditable,
            bool isAdmin = false,
            bool isActive = true,
            IReadOnlyList<string> roles = null,
            bool locked = false,
            bool emailConfirmed = false,
            string photoUrl = null,
            string status = null,
            string createdBy = null,
            DateTime? createdDate = null,
            string modifiedBy = null,
            DateTime? modifiedDate = null)
        {
            CheckEmailValidity(email);
            CheckNameValidity(name);
            CheckingRoleValidity(roles);

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
            _roles = roles?.Select(x => Role.Of(x, x)).ToList();
            _permissions = permissions?.Select(x => Permission.Of(x, "")).ToList() ?? new List<Permission>();
            EmailConfirmed = emailConfirmed;
            PhotoUrl = photoUrl;
            Status = status;
            IsAdministrator = isAdmin;
            IsActive = isActive;
            _userEditable = userEditable;
            CreatedDate = createdDate ?? DateTime.Now;
            CreatedBy = createdBy;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;

            AddDomainEvent(new NewUserRegisteredDomainEvent(this));
        }


        public static User Of(UserId id,
            string email,
            string firstName,
            string lastName,
            string name,
            string userName,
            string password,
            IReadOnlyList<string> permissions,
            UserType userType,
            IUserEditable userEditable,
            bool isAdmin = false,
            bool isActive = true,
            IReadOnlyList<string> roles = null,
            bool locked = false,
            bool emailConfirmed = false,
            string photoUrl = null,
            string status = null,
            string createdBy = null,
            DateTime? createdDate = null,
            string modifiedBy = null,
            DateTime? modifiedDate = null)
        {
            return new(id,
                email,
                firstName,
                lastName,
                name,
                userName,
                password,
                permissions,
                userType,
                userEditable,
                isAdmin,
                isActive,
                roles,
                locked,
                emailConfirmed,
                photoUrl,
                status,
                createdBy,
                createdDate,
                modifiedBy,
                modifiedDate);
        }

        public void AssignRole(Role role)
        {
            if (role is null)
                throw new Exception("Role can't be null.");

            var exists = _roles.Contains(role);

            if (!exists) _roles.Add(role);
        }

        public virtual void Patch(User target)
        {
        }

        public void CheckUserEditable()
        {
            CheckRule(new UserEditableRule(UserName, _userEditable));
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

        public static void CheckingRoleValidity(IReadOnlyList<string> roles)
        {
            roles?.ToList().ForEach(role =>
            {
                if (!Role.IsValid(role)) throw new InvalidRoleException(role);
            });
        }

        public static void CheckNameValidity(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidNameException(name);
        }

        #endregion
    }
}