using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.DomainEvents;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.Repositories;

namespace OnlineStore.Modules.Identity.Application.ConfirmUserRegistration
{
    public class UserRegistrationConfirmedHandler : IDomainEventHandler<UserRegistrationConfirmedDomainEvent>
    {
        private readonly IUserRegistrationRepository _userRegistrationRepository;

        private readonly IUserRepository _userRepository;

        public UserRegistrationConfirmedHandler(
            IUserRegistrationRepository userRegistrationRepository,
            IUserRepository userRepository)
        {
            _userRegistrationRepository = userRegistrationRepository;
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UserRegistrationConfirmedDomainEvent message)
        {
            var userRegistration = await _userRegistrationRepository.GetByIdAsync(message.UserRegistrationId);

            var user = userRegistration.CreateUser();

            await _userRepository.AddAsync(user);
        }
    }
}