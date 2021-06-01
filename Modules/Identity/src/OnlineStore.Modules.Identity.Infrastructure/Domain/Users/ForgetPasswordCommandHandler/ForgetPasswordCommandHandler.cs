using System.Text;
using System.Threading.Tasks;
using Common.Messaging.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using OnlineStore.Modules.Identity.Application.Features.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Features.Users.ForgotPassword;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.ForgetPasswordCommandHandler
{
    public class ForgetPasswordCommandHandler:ICommandHandler<ForgetPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task HandleAsync(ForgetPasswordCommand command)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                throw new UserNotFoundException(command.Email);
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = $"{command.RequestScheme}://{command.RequestHost}/users/reset-password/{user.Id}/{code}";

            //TODO: Send Email
            // await _emailSender.SendEmailAsync(user.Email, "Forget Password", callbackUrl.ToString());
        }
    }
}