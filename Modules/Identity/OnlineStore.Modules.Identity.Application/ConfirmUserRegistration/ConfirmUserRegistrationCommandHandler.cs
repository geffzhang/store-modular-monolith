using System.Threading.Tasks;
using Common.Messaging.Commands;
using OnlineStore.Modules.Identity.Domain.UserRegistrations;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.Repositories;

namespace OnlineStore.Modules.Identity.Application.ConfirmUserRegistration
{
    internal class ConfirmUserRegistrationCommandHandler : ICommandHandler<ConfirmUserRegistrationCommand>
    {
        private readonly IUserRegistrationRepository _userRegistrationRepository;

        public ConfirmUserRegistrationCommandHandler(IUserRegistrationRepository userRegistrationRepository)
        {
            _userRegistrationRepository = userRegistrationRepository;
        }
        public async Task HandleAsync(ConfirmUserRegistrationCommand command)
        {
            var userRegistration =
                await _userRegistrationRepository.GetByIdAsync(new UserRegistrationId(command.UserRegistrationId));

            userRegistration.Confirm();
        }
    }
}