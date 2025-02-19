using System;
using System.Collections.Generic;
using BuildingBlocks.Cqrs.Commands;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Types;

namespace OnlineStore.Modules.Identity.Application.Users.Features.UpdateUser
{
    public class UpdateUserCommand : ICommand
    {
        public UpdateUserCommand(Guid id, string email, string firstName, string lastName,
            string name, string userName, string phoneNumber, string password, DateTime createdDate, string createdBy,
            IReadOnlyList<string> permissions, UserType userType, bool isAdmin = false, bool isActive = true,
            IReadOnlyList<string> roles = null, bool locked = false, bool emailConfirmed = false,
            string photoUrl = null, string status = null, string modifiedBy = null,
            DateTime? modifiedDate = null)
        {
            UserName = userName;
            IsActive = true;
            Id = id;
            Email = email.ToLowerInvariant();
            FirstName = firstName;
            LastName = lastName;
            Name = name.Trim();
            UserType = userType;
            PhoneNumber = phoneNumber;
            Password = password;
            CreatedDate = createdDate;
            LockoutEnabled = locked;
            Roles = roles;
            Permissions = permissions;
            EmailConfirmed = emailConfirmed;
            PhotoUrl = photoUrl;
            Status = status;
            CreatedBy = createdBy;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            IsAdministrator = isAdmin;
            IsActive = isActive;
        }

        public string UserName { get; }
        public bool EmailConfirmed { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Name { get; }
        public string PhoneNumber { get; }
        public bool IsAdministrator { get; }
        public string PhotoUrl { get; }
        public UserType UserType { get; }
        public string Status { get; }
        public string Password { get; }
        public DateTime CreatedDate { get; }
        public DateTime? ModifiedDate { get; }
        public string CreatedBy { get; }
        public string ModifiedBy { get; }
        public bool LockoutEnabled { get; }
        public bool IsActive { get; }
        public IEnumerable<string> Roles { get; }
        public IEnumerable<string> Permissions { get; }
        public Guid Id { get; }
    }
}