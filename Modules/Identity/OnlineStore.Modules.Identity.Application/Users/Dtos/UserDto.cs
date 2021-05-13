using System;
using System.Collections.Generic;
using OnlineStore.Modules.Identity.Domain.Users.Types;

namespace OnlineStore.Modules.Identity.Application.Users.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string MemberId { get; set; }
        public bool IsAdministrator { get; set; }
        public string PhotoUrl { get; set; }
        public UserType UserType { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool IsActive { get; set; }
        public IReadOnlyList<string> Roles { get; set; }
        public IReadOnlyList<string> Permissions { get; set; }
    }
}