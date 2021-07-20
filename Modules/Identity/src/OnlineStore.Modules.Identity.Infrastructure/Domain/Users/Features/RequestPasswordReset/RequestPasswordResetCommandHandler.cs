using System.Threading.Tasks;
using Common.Core.Messaging.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Features.Users.RequestPasswordReset;
using OnlineStore.Modules.Identity.Domain.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Features.RequestPasswordReset
{
    public class RequestPasswordResetCommandHandler : ICommandHandler<RequestPasswordResetCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserEditable _userEditable;

        public RequestPasswordResetCommandHandler(UserManager<ApplicationUser> userManager, IUserEditable userEditable)
        {
            _userManager = userManager;
            _userEditable = userEditable;
        }

        public async Task HandleAsync(RequestPasswordResetCommand command)
        {
            var user = await _userManager.FindByNameAsync(command.LoginOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(command.LoginOrEmail);
            }

            //Do not permit rejected users and customers
            if (user?.Email != null && _userEditable.IsUserEditable(user.UserName) &&
                !(await _userManager.IsInRoleAsync(user, SecurityConstants.SystemRoles.Customer)))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = $"{command.RequestScheme}://{command.RequestHost}/#/resetpassword/{user.Id}/{token}";

                //TODO: Send Email
                // await _emailSender.SendEmailAsync(user.Email, "Reset password", callbackUrl.ToString());
            }
        }
    }
}