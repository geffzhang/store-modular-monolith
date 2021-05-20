using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Common.Messaging.Commands;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Domain.Users.Services;

namespace OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser
{
    public class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserEditable _userEditable;

        public RegisterNewUserCommandHandler(IUserRepository userRepository, IUserEditable userEditable)
        {
            _userRepository = userRepository;
            _userEditable = userEditable;
        }

        public async Task HandleAsync(RegisterNewUserCommand command)
        {
            Guard.Against.Null(command, nameof(RegisterNewUserCommand));

            await _userRepository.AddAsync(User.Of(command.Id, command.Email, command.FirstName, command.LastName,
                command.Name, command.UserName, command.Password, command.CreatedDate, command.CreatedBy,
                command.Permissions.ToList(), command.UserType, _userEditable, command.IsAdministrator,
                command.IsActive,
                command.Roles.ToList(), command.LockoutEnabled, command.EmailConfirmed, command.PhotoUrl,
                command.Status, command.ModifiedBy, command.ModifiedDate));
        }
    }
}