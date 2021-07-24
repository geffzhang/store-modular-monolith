using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging.Commands;
using BuildingBlocks.Core.Types;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Authentication.Login;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Authentication.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task HandleAsync(LoginCommand command)
        {
            // var user = await _userManager.FindByEmailAsync(command.Email);
            // if (user == null)
            // {
            //     return await Result<TokenResponse>.FailAsync(_localizer["User Not Found."]);
            // }
            //
            // if (!user.IsActive)
            // {
            //     return await Result<TokenResponse>.FailAsync(
            //         _localizer["User Not Active. Please contact the administrator."]);
            // }
            //
            // if (!user.EmailConfirmed)
            // {
            //     return await Result<TokenResponse>.FailAsync(_localizer["E-Mail not confirmed."]);
            // }
            //
            // var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            // if (!passwordValid)
            // {
            //     return await Result<TokenResponse>.FailAsync(_localizer["Invalid Credentials."]);
            // }
            //
            // user.RefreshToken = GenerateRefreshToken();
            // user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            // await _userManager.UpdateAsync(user);
            //
            // var token = await GenerateJwtAsync(user);
            // var response = new TokenResponse
            //     {Token = token, RefreshToken = user.RefreshToken, UserImageURL = user.ProfilePictureDataUrl};
            // return await Result<TokenResponse>.SuccessAsync(response);
        }
    }
}