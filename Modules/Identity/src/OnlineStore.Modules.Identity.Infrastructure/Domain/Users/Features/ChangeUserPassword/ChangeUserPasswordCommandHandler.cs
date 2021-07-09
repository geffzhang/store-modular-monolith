using System.Threading.Tasks;
using Common.Messaging.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Features.Users.ChangeUserPassword;
using OnlineStore.Modules.Identity.Application.Features.Users.Exceptions;
using OnlineStore.Modules.Identity.Domain.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Features.ChangeUserPassword
{
    public class ChangeCurrentUserPasswordCommandHandler : ICommandHandler<ChangeUserPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserEditable _userEditable;

        public ChangeCurrentUserPasswordCommandHandler(UserManager<ApplicationUser> userManager,
            IUserEditable userEditable)
        {
            _userManager = userManager;
            _userEditable = userEditable;
        }

        public async Task HandleAsync(ChangeUserPasswordCommand command)
        {
            var appuser = await _userManager.FindByNameAsync(command.UserName);
            if (appuser == null)
            {
                throw new UserNotFoundException(command.UserName);
            }

            if (_userEditable.IsUserEditable(command.UserName) == false)
            {
                throw new UserCanNotEditException(command.UserName);
            }

            if (command.OldPassword == command.NewPassword)
            {
                throw new PasswordShouldNotEqualOldException();
            }

            var result = await _userManager.ChangePasswordAsync(appuser, command.OldPassword,
                command.NewPassword);
            if (result.Succeeded && appuser.PasswordExpired)
            {
                appuser.PasswordExpired = false;
                await _userManager.UpdateAsync(appuser);
            }
            else
            {
                throw new ChangeUserPasswordFailedException(command.UserName);
            }
        }
    }
}