using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Modules.Identity.Api.Authentication.Models;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Api.Authentication
{
    [ApiController]
    [Route("externalsignin")]
    public class ExternalSignInController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommandProcessor _commandProcessor;

        public ExternalSignInController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ICommandProcessor commandProcessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _commandProcessor = commandProcessor;
        }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public ActionResult SignIn(string authenticationType)
        {
            var returnUrl = Url.Action("Index", "Home");
            var callbackUrl = Url.Action("SignInCallback", "ExternalSignIn", new {returnUrl});

            var authenticationProperties = new AuthenticationProperties {RedirectUri = callbackUrl};
            authenticationProperties.Items["LoginProvider"] = authenticationType;

            return Challenge(authenticationProperties, authenticationType);
        }

        [HttpGet]
        [Route("callback")]
        [AllowAnonymous]
        public async Task<ActionResult> SignInCallback(string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ApplicationUser user = null;

            //try yo take an user name from claims
            var userName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Upn) ??
                           externalLoginInfo.Principal.FindFirstValue("preferred_username");
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new InvalidOperationException(
                    "Received external login info does not have an UPN claim or DefaultUserName.");
            }

            var externalLoginResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, false);
            if (!externalLoginResult.Succeeded)
            {
                //Need handle the two cases
                //first - when the user account already exists, it is just missing an external login info and
                //second - when user does not have an account, then create a new account for them
                user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = userName,
                        Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ??
                                (userName.IsValidEmail() ? userName : null)
                        // TODO: somehow access the AzureAd configuration section and read the default user type from there
                        //UserType = _authenticationOptions.AzureAdDefaultUserType
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        var joinedErrors = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
                        throw new InvalidOperationException("Failed to save a account due the errors: " +
                                                            joinedErrors);
                    }
                }

                var newExternalLogin = new UserLoginInfo(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey,
                    externalLoginInfo.ProviderDisplayName);

                await _userManager.AddLoginAsync(user, newExternalLogin);

                //SignIn  user in the system
                var aspNetUser = await _signInManager.UserManager.FindByNameAsync(user.UserName);
                await _signInManager.SignInAsync(aspNetUser, isPersistent: true);
            }
            else if (externalLoginResult.IsLockedOut || externalLoginResult.RequiresTwoFactor)
            {
                // TODO: handle user lock-out and two-factor authentication
                return RedirectToAction("Index", "Home");
            }

            if (user == null) user = await _userManager.FindByNameAsync(userName);

            await _commandProcessor.PublishDomainEventAsync(new UserLoginEvent(user));

            return Redirect(returnUrl);
        }

        [HttpGet]
        [Route("providers")]
        [AllowAnonymous]
        public async Task<ActionResult> GetExternalLoginProviders()
        {
            var externalLoginProviders = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Select(authenticationDescription => new ExternalSignInProviderInfo
                {
                    AuthenticationType = authenticationDescription.Name,
                    DisplayName = authenticationDescription.DisplayName
                })
                .ToArray();

            return Ok(externalLoginProviders);
        }
    }
}