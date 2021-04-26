using System;
using Common.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.RegisterNewUser
{
    public class RegisterNewUserCommand : ICommand
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CorrelationId { get; set; }
        
        public string Login { get; }

        public string Password { get; }

        public string Email { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string ConfirmLink { get; }

        public AddressDto Address { get; }
        
        public RegisterNewUserCommand(
            string login,
            string password,
            string email,
            string firstName,
            string lastName,
            string confirmLink,
            AddressDto address)
        {
            Login = login;
            Password = password;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            ConfirmLink = confirmLink;
            Address = address;
        }

    }
}