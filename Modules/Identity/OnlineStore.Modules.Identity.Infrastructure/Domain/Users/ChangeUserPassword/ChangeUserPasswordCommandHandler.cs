using System.Threading.Tasks;
using Common.Messaging.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Infrastructure.Extensions;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.ChangeCurrentUserPassword
{
    public class ChangeCurrentUserPasswordCommandHandler : ICommandHandler<ChangeCurrentUserPasswordCommand>
    {
        private readonly IUserRepository _userRepository;

        public ChangeCurrentUserPasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task HandleAsync(ChangeCurrentUserPasswordCommand command)
        {
            var user = await _userRepository.FindByNameAsync(command.UserName);

            if (user == null)
            {
                throw new UserNotFoundException(command.UserName);
            }

            user.CheckUserEditable();

            if (command.OldPassword == command.NewPassword)
            {
                return BadRequest(new ChangePasswordResult
                {
                    Errors = new[] {"You have used this password in the past. Choose another one."}
                });
            }

            var result =
                await _signInManager.UserManager.ChangePasswordAsync(user, changePassword.OldPassword,
                    changePassword.NewPassword);
            if (result.Succeeded && user.PasswordExpired)
            {
                user.PasswordExpired = false;
                await _userManager.UpdateAsync(user);
            }
        }
    }
}