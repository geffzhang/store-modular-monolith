using System.Threading.Tasks;
using Common.Messaging.Commands;
using OnlineStore.Modules.Identity.Application.Authentication;
using OnlineStore.Modules.Identity.Domain;
using OnlineStore.Modules.Identity.Domain.UserRegistrations;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.DomainServices;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.Repositories;

namespace OnlineStore.Modules.Identity.Application.RegisterNewUser
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
                Address.Of(command.Address.Street, command.Address.City, command.Address.State, command.Address.Country,
                    command.Address.ZipCode),
                _usersCounter,
                command.ConfirmLink);

            await _userRegistrationRepository.AddAsync(userRegistration);
        }
    }
}