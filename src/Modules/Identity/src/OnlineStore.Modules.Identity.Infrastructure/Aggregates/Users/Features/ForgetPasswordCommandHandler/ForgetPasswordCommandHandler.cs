using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Users.Features.ForgotPassword;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.ForgetPasswordCommandHandler
{
    public class ForgetPasswordCommandHandler:ICommandHandler<ForgetPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Unit> HandleAsync(ForgetPasswordCommand command,CancellationToken cancellationToken = default)
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

            return Unit.Result;
        }
    }
}