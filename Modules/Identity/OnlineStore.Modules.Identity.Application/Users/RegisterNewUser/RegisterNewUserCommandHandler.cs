using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Commands;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Users.RegisterNewUser
{
    public class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public RegisterNewUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(RegisterNewUserCommand command)
        {
            await _userRepository.AddAsync(User.Of(command.Id, command.Email, command.FirstName, command.LastName,
                command.Name, command.UserName, command.Password, command.CreatedDate, command.CreatedBy,
                command.Permissions.ToList(), command.UserType, command.IsAdministrator, command.IsActive,
                command.Roles.ToList(), command.LockoutEnabled, command.EmailConfirmed, command.PhotoUrl,
                command.Status, command.ModifiedBy, command.ModifiedDate));
        }
    }
}