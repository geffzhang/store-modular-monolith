using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users.Features.RequestPasswordReset;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.RequestPasswordReset
{
    public class RequestPasswordResetCommandHandler : ICommandHandler<RequestPasswordResetCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestPasswordResetCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> HandleAsync(RequestPasswordResetCommand command,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(command.LoginOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(command.LoginOrEmail);
            }

            //Do not permit rejected users and customers
            if (user?.Email != null &&
                !(await _userManager.IsInRoleAsync(user, SecurityConstants.SystemRoles.Customer)))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = $"{command.RequestScheme}://{command.RequestHost}/#/resetpassword/{user.Id}/{token}";

                //TODO: Send Email
                // await _emailSender.SendEmailAsync(user.Email, "Reset password", callbackUrl.ToString());
            }

            return Unit.Result;
        }
    }
}