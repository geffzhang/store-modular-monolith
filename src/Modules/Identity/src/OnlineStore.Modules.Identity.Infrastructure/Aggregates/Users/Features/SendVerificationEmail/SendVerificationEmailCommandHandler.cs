using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Mail;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using OnlineStore.Modules.Identity.Application.Users.Features.SendVerificationEmail;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.SendVerificationEmail
{
    public class SendVerificationEmailCommandHandler : ICommandHandler<SendVerificationEmailCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailService _mailService;

        public SendVerificationEmailCommandHandler(UserManager<ApplicationUser> userManager, IMailService mailService)
        {
            _userManager = userManager;
            _mailService = mailService;
        }

        public async Task<Unit> HandleAsync(SendVerificationEmailCommand command,
            CancellationToken cancellationToken = default)
        {
            var applicationUser = (await _userManager.FindByIdAsync(command.UserId));
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl =
                $"{command.RequestScheme}://{command.RequestHost}/#/confirm-email/{applicationUser.Id}/{encodedCode}";

            string link = $"<a href='{callbackUrl}'>link</a>";
            string content =
                $"Welcome to Online Shopping application! Please confirm your registration using this {link}.";

            // Send Email
            await _mailService.SendAsync(new MailRequest(applicationUser.Email, "Confirmation Email", content));

            return Unit.Result;
        }
    }
}