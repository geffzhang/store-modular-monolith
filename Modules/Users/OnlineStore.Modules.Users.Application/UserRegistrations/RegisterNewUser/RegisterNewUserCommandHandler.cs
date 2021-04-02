using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Messaging.Commands;
using OnlineStore.Modules.Users.Application.Authentication;
using OnlineStore.Modules.Users.Domain.UserRegistrations;
using OnlineStore.Modules.Users.Domain.UserRegistrations.DomainServices;

namespace OnlineStore.Modules.Users.Application.UserRegistrations.RegisterNewUser
{
    internal class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand>
    {
        private readonly IUserRegistrationRepository _userRegistrationRepository;
        private readonly IUsersCounter _usersCounter;

        public RegisterNewUserCommandHandler(
            IUserRegistrationRepository userRegistrationRepository,
            IUsersCounter usersCounter)
        {
            _userRegistrationRepository = userRegistrationRepository;
            _usersCounter = usersCounter;
        }

        public async Task HandleAsync(RegisterNewUserCommand command)
        {
            var password = PasswordManager.HashPassword(command.Password);

            var userRegistration = UserRegistration.RegisterNewUser(
                command.Login,
                password,
                command.Email,
                command.FirstName,
                command.LastName,
                _usersCounter);

            await _userRegistrationRepository.AddAsync(userRegistration);
        }
    }
}