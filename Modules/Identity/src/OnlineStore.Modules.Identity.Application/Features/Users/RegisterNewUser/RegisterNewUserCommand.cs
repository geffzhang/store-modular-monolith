using System;
using System.Collections.Generic;
using Common.Messaging.Commands;
using OnlineStore.Modules.Identity.Domain.Users.Types;

namespace OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser
{
    public class RegisterNewUserCommand : ICommand
    {
        public RegisterNewUserCommand(Guid id, string email, string firstName, string lastName,
            string name, string userName, string password, 
            IReadOnlyList<string> permissions, UserType userType, bool isAdmin = false, bool isActive = true,
            IReadOnlyList<string>? roles = null, bool locked = false, bool emailConfirmed = false,
            string? photoUrl = null, string? status = null)
        {
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
            Roles = roles;
            Permissions = permissions;
            EmailConfirmed = emailConfirmed;
            PhotoUrl = photoUrl;
            Status = status;
            IsAdministrator = isAdmin;
            IsActive = isActive;
        }
        public string UserName { get; }
        public bool EmailConfirmed { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Name { get; }
        public bool IsAdministrator { get; }
        public string? PhotoUrl { get; }
        public UserType UserType { get; }
        public string? Status { get; }
        public string Password { get; }
        public bool LockoutEnabled { get; }
        public bool IsActive { get; }
        public IEnumerable<string>? Roles { get; }
        public IEnumerable<string>? Permissions { get; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}