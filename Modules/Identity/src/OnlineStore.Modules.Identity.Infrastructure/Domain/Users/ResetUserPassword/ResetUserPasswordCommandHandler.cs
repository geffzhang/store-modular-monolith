using System.Threading.Tasks;
using Common.Messaging.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Features.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Features.Users.ResetUserPassword;
using OnlineStore.Modules.Identity.Domain.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.ResetUserPassword
{
    public class ResetUserPasswordCommandHandler : ICommandHandler<ResetUserPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserEditable _userEditable;

        public ResetUserPasswordCommandHandler(UserManager<ApplicationUser> userManager, IUserEditable userEditable)
        {
            _userManager = userManager;
            _userEditable = userEditable;
        }

        public async Task HandleAsync(ResetUserPasswordCommand command)
        {
            ApplicationUser appuser = null;
            if (string.IsNullOrEmpty(command.UserName) == false)
            {
                appuser = await _userManager.FindByNameAsync(command.UserName);
            }

            if (string.IsNullOrEmpty(command.UserId) == false)
            {
                appuser = await _userManager.FindByIdAsync(command.UserId);
            }

            if (appuser == null)
            {
                throw new UserNotFoundException(command.UserName);
            }

            if (_userEditable.IsUserEditable(command.UserName) == false)
            {
                throw new UserCanNotEditException(command.UserName);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(appuser);
            var result = await _userManager.ResetPasswordAsync(appuser, token, command.NewPassword);
            if (result.Succeeded)
            {
                appuser = await _userManager.FindByNameAsync(command.UserName);

                if (appuser.PasswordExpired != command.ForcePasswordChangeOnNextSignIn)
                {
                    appuser.PasswordExpired = command.ForcePasswordChangeOnNextSignIn;

                    await _userManager.UpdateAsync(appuser);
                }
            }
            else
            {
                throw new ResetPasswordFailedException(command.UserId);
            }
        }
    }
}