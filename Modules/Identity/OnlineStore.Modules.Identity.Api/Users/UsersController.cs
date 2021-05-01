using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Validation;
using AspNet.Security.OpenIdConnect.Primitives;
using Common;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OnlineStore.Modules.Identity.Api.Models;
using OnlineStore.Modules.Identity.Application.Authentication.Dtos;
using OnlineStore.Modules.Identity.Application.Search;
using OnlineStore.Modules.Identity.Application.Search.Dtos;
using OnlineStore.Modules.Identity.Domain.Permissions;
using OnlineStore.Modules.Identity.Domain.Users.DomainEvents;
using OnlineStore.Modules.Identity.Infrastructure;
using OnlineStore.Modules.Identity.Infrastructure.Authentication;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Mappings;
using OpenIddict.Abstractions;

namespace OnlineStore.Modules.Identity.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly Infrastructure.Authorization.AuthorizationOptions
            _securityOptions;
        private readonly UserOptionsExtended _userOptionsExtended;
        private readonly IPermissionService _permissionsProvider;
        private readonly IUserSearchService _userSearchService;
        private readonly IRoleSearchService _roleSearchService;
        private readonly IPasswordValidator<ApplicationUser> _passwordCheckService;
        private readonly ICommandProcessor _commandProcessor;

        public UsersController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IPermissionService permissionsProvider, IUserSearchService userSearchService,
            IRoleSearchService roleSearchService,
            IOptions<Infrastructure.Authorization.AuthorizationOptions> securityOptions,
            IOptions<UserOptionsExtended> userOptionsExtended,
            IPasswordValidator<ApplicationUser> passwordCheckService,
            IAuthorizationService authorizationService,
            ICommandProcessor commandProcessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _securityOptions = securityOptions.Value;
            _userOptionsExtended = userOptionsExtended.Value;
            _passwordCheckService = passwordCheckService;
            _permissionsProvider = permissionsProvider;
            _roleManager = roleManager;
            _userSearchService = userSearchService;
            _roleSearchService = roleSearchService;
            _commandProcessor = commandProcessor;
            _authorizationService = authorizationService;
        }


        /// <summary>
        /// Get current user details
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("currentuser")]
        public async Task<ActionResult<UserDetail>> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return NotFound();
            }

            var result = new UserDetail
            {
                Id = user.Id,
                IsAdministrator = user.IsAdministrator,
                UserName = user.UserName,
                PasswordExpired = user.PasswordExpired,
                DaysTillPasswordExpiry =
                    PasswordExpiryHelper.ContDaysTillPasswordExpiry(user, _userOptionsExtended),
                Permissions = user.Roles.SelectMany(x => x.Permissions).Select(x => x.Name).Distinct().ToArray()
            };

            return Ok(result);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [Route("userinfo")]
        public async Task<ActionResult<Claim[]>> Userinfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The user profile is no longer available."
                });
            }

            var claims = new JObject
            {
                //TODO: replace to PrinciplaClaims

                // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
                [OpenIdConnectConstants.Claims.Subject] = await _userManager.GetUserIdAsync(user)
            };

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIdConnectConstants.Scopes.Email))
            {
                claims[OpenIdConnectConstants.Claims.Email] = await _userManager.GetEmailAsync(user);
                claims[OpenIdConnectConstants.Claims.EmailVerified] = await _userManager.IsEmailConfirmedAsync(user);
            }

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIdConnectConstants.Scopes.Phone))
            {
                claims[OpenIdConnectConstants.Claims.PhoneNumber] = await _userManager.GetPhoneNumberAsync(user);
                claims[OpenIdConnectConstants.Claims.PhoneNumberVerified] =
                    await _userManager.IsPhoneNumberConfirmedAsync(user);
            }

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIddictConstants.Scopes.Roles))
            {
                claims["roles"] = JArray.FromObject(await _userManager.GetRolesAsync(user));
            }

            // Note: the complete list of standard claims supported by the OpenID Connect specification
            // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

            return new JsonResult(claims);
        }

        /// <summary>
        /// SearchAsync users by keyword
        /// </summary>
        /// <param name="criteria">Search criteria.</param>
        [HttpPost]
        [Route("users/search")]
        [Route("users")] //PT-789: Obsolete, remove later left only for backward compatibility with V2
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult<UserSearchResult>> SearchUsers([FromBody] UserSearchCriteria criteria)
        {
            var result = await _userSearchService.SearchUsersAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Get user details by user name
        /// </summary>
        /// <param name="userName"></param>
        [HttpGet]
        [Route("users/{userName}")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult<ApplicationUser>> GetUserByName([FromRoute] string userName)
        {
            var retVal = await _userManager.FindByNameAsync(userName);
            return Ok(retVal);
        }

        /// <summary>
        /// Get user details by user ID
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [Route("users/id/{id}")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult<ApplicationUser>> GetUserById([FromRoute] string id)
        {
            var retVal = await _userManager.FindByIdAsync(id);
            return Ok(retVal);
        }

        /// <summary>
        /// Get user details by user email
        /// </summary>
        /// <param name="email"></param>
        [HttpGet]
        [Route("users/email/{email}")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult<ApplicationUser>> GetUserByEmail([FromRoute] string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            return Ok(result);
        }

        /// <summary>
        /// Get user details by external login provider
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("users/login/external/{loginProvider}/{providerKey}")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult<ApplicationUser>> GetUserByLogin([FromRoute] string loginProvider,
            [FromRoute] string providerKey)
        {
            var result = await _userManager.FindByLoginAsync(loginProvider, providerKey);
            return Ok(result);
        }


        // // POST: api/account/user
        // [HttpPost("user")]
        // [Authorize(SecurityConstants.Permissions.CanCreateUsers)]
        // [ValidateAntiForgeryToken]
        // public async Task<ActionResult<UserActionIdentityResult>> RegisterUser(
        //     [FromBody] RegisterNewUserRequest registration)
        // {
        //     UserActionIdentityResult result;
        //
        //     TryValidateModel(registration);
        //
        //     if (ModelState.IsValid)
        //     {
        //         //Allow to register new users only within own organization
        //         var authorizationResult = await _authorizationService.AuthorizeAsync(User,
        //             registration,
        //             CanEditOrganizationResourceAuthorizeRequirement.PolicyName);
        //         if (!authorizationResult.Succeeded)
        //         {
        //             return Unauthorized();
        //         }
        //
        //         var contact = registration.ToContact();
        //         contact.OrganizationId = registration.OrganizationId;
        //
        //         var user = registration.ToUser();
        //         user.Contact = contact;
        //         user.StoreId = string.Empty;
        //
        //         // user.Roles = new[]
        //         // {
        //         //     SecurityConstants.Roles.Customer
        //         // };
        //
        //         var creatingResult = await _userManager.CreateAsync(user, registration.Password);
        //         result = UserActionIdentityResult.Instance(creatingResult);
        //         if (result.Succeeded)
        //         {
        //             user = await _userManager.FindByNameAsync(user.UserName);
        //             await _commandProcessor.PublishDomainEventAsync(new UserRegisteredEvent(WorkContext, user,
        //                 registration));
        //             result.MemberId = user.Id;
        //         }
        //     }
        //     else
        //     {
        //         result = UserActionIdentityResult
        //             .Failed(ModelState.Values.SelectMany(x => x.Errors)
        //                 .Select(x => new IdentityError {Description = x.ErrorMessage})
        //                 .ToArray());
        //     }
        //
        //     return result;
        // }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="newUser"></param>
        [HttpPost]
        [Route("users/create")]
        [Authorize(SecurityConstants.Permissions.SecurityCreate)]
        public async Task<ActionResult<SecurityResult>> Create([FromBody] ApplicationUser newUser)
        {
            IdentityResult result;

            if (string.IsNullOrEmpty(newUser.Password))
            {
                result = await _userManager.CreateAsync(newUser);
            }
            else
            {
                result = await _userManager.CreateAsync(newUser, newUser.Password);
            }

            return Ok(IdentityResultExtensions.ToSecurityResult(result));
        }

        /// <summary>
        /// Change password for current user.
        /// </summary>
        /// <param name="changePassword">Old and new passwords.</param>
        /// <returns>Result of password change</returns>
        [HttpPost]
        [Route("currentuser/changepassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<SecurityResult>> ChangeCurrentUserPassword(
            [FromBody] ChangePasswordRequest changePassword)
        {
            return await ChangePassword(User.Identity.Name, changePassword);
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="changePassword">Old and new passwords.</param>
        [HttpPost]
        [Route("users/{userName}/changepassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult<SecurityResult>> ChangePassword([FromRoute] string userName,
            [FromBody] ChangePasswordRequest changePassword)
        {
            if (!IsUserEditable(userName))
            {
                return BadRequest(IdentityResultExtensions.ToSecurityResult(IdentityResult
                    .Failed(new IdentityError {Description = "It is forbidden to edit this user."})));
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return BadRequest(IdentityResultExtensions.ToSecurityResult(
                    IdentityResult.Failed(new IdentityError {Description = "User not found."})));
            }

            if (changePassword.OldPassword == changePassword.NewPassword)
            {
                return BadRequest(new SecurityResult
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

            return Ok(IdentityResultExtensions.ToSecurityResult(result));
        }

        /// <summary>
        /// Resets password for current user.
        /// </summary>
        /// <param name="resetPassword">Password reset information containing new password.</param>
        /// <returns>Result of password reset.</returns>
        [HttpPost]
        [Route("currentuser/resetpassword")]
        [AllowAnonymous]
        [Obsolete("use /currentuser/changepassword instead")]
        public async Task<ActionResult<SecurityResult>> ResetCurrentUserPassword(
            [FromBody] ResetPasswordConfirmRequest resetPassword)
        {
            return await ResetPassword(User.Identity.Name, resetPassword);
        }

        /// <summary>
        /// Reset password confirmation
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="resetPasswordConfirm">New password.</param>
        [HttpPost]
        [Route("users/{userName}/resetpassword")]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult<SecurityResult>> ResetPassword([FromRoute] string userName,
            [FromBody] ResetPasswordConfirmRequest resetPasswordConfirm)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return BadRequest(
                    IdentityResultExtensions.ToSecurityResult(
                        IdentityResult.Failed(new IdentityError {Description = "User not found"})));
            }

            if (!IsUserEditable(user.UserName))
            {
                return BadRequest(IdentityResultExtensions.ToSecurityResult(IdentityResult
                    .Failed(new IdentityError {Description = "It is forbidden to edit this user."})));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordConfirm.NewPassword);
            if (result.Succeeded)
            {
                user = await _userManager.FindByNameAsync(userName);

                if (user.PasswordExpired != resetPasswordConfirm.ForcePasswordChangeOnNextSignIn)
                {
                    user.PasswordExpired = resetPasswordConfirm.ForcePasswordChangeOnNextSignIn;

                    await _userManager.UpdateAsync(user);
                }
            }

            return Ok(IdentityResultExtensions.ToSecurityResult(result));
        }

        /// <summary>
        /// Reset password confirmation
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resetPasswordConfirm">New password.</param>
        [HttpPost]
        [Route("users/{userId}/resetpasswordconfirm")]
        [AllowAnonymous]
        public async Task<ActionResult<SecurityResult>> ResetPasswordByToken([FromRoute] string userId,
            [FromBody] ResetPasswordConfirmRequest resetPasswordConfirm)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(
                    IdentityResultExtensions.ToSecurityResult(
                        IdentityResult.Failed(new IdentityError {Description = "User not found"})));
            }

            if (!IsUserEditable(user.UserName))
            {
                return BadRequest(IdentityResultExtensions.ToSecurityResult(IdentityResult
                    .Failed(new IdentityError {Description = "It is forbidden to edit this user."})));
            }

            var result = await _signInManager.UserManager.ResetPasswordAsync(user, resetPasswordConfirm.Token,
                resetPasswordConfirm.NewPassword);
            if (result.Succeeded && user.PasswordExpired)
            {
                user.PasswordExpired = false;

                await _userManager.UpdateAsync(user);
            }

            return Ok(IdentityResultExtensions.ToSecurityResult(result));
        }

        /// <summary>
        /// Validate password reset token
        /// </summary>
        [HttpPost]
        [Route("users/{userId}/validatepasswordresettoken")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> ValidatePasswordResetToken(string userId,
            [FromBody] ValidatePasswordResetTokenRequest resetPasswordToken)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);
            var tokenProvider = _userManager.Options.Tokens.PasswordResetTokenProvider;
            var result = await _userManager.VerifyUserTokenAsync(applicationUser, tokenProvider, "ResetPassword",
                resetPasswordToken.Token);
            return Ok(result);
        }

        /// <summary>
        /// Send email with instructions on how to reset user password.
        /// </summary>
        /// <remarks>
        /// Verifies provided userName and (if succeeded) sends email.
        /// </remarks>
        [HttpPost]
        [Route("users/{loginOrEmail}/requestpasswordreset")]
        [AllowAnonymous]
        public async Task<ActionResult> RequestPasswordReset(string loginOrEmail)
        {
            var user = await _userManager.FindByNameAsync(loginOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(loginOrEmail);
            }

            //Do not permit rejected users and customers
            if (user?.Email != null && IsUserEditable(user.UserName) &&
                !(await _userManager.IsInRoleAsync(user, SecurityConstants.SystemRoles.Customer)))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = $"{Request.Scheme}://{Request.Host}/#/resetpassword/{user.Id}/{token}";

                // await _emailSender.SendEmailAsync(user.Email, "Reset password", callbackUrl.ToString());
            }

            return Ok();
        }

        /// <summary>
        /// Update user details by user ID
        /// </summary>
        /// <param name="user">User details.</param>
        [HttpPut]
        [Route("users")]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult<SecurityResult>> Update([FromBody] ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!IsUserEditable(user.UserName))
            {
                return Ok(IdentityResultExtensions.ToSecurityResult(
                    IdentityResult.Failed(new IdentityError {Description = "It is forbidden to edit this user."})));
            }

            var applicationUser = await _userManager.FindByIdAsync(user.Id);
            if (applicationUser.EmailConfirmed != user.EmailConfirmed
                && !Request.HttpContext.User.HasGlobalPermission(SecurityConstants.Permissions.SecurityVerifyEmail))
            {
                return Unauthorized();
            }

            if (!applicationUser.Email.EqualsInvariant(user.Email))
            {
                // SetEmailAsync also: sets EmailConfirmed to false and updates the SecurityStamp
                await _userManager.SetEmailAsync(user, user.Email);
            }

            if (user.LastPasswordChangedDate != applicationUser.LastPasswordChangedDate)
            {
                user.LastPasswordChangedDate = applicationUser.LastPasswordChangedDate;
            }

            var result = await _userManager.UpdateAsync(user);

            return Ok(IdentityResultExtensions.ToSecurityResult(result));
        }

        /// <summary>
        /// Delete users by name
        /// </summary>
        /// <param name="names">An array of user names.</param>
        [HttpDelete]
        [Route("users")]
        [Authorize(SecurityConstants.Permissions.SecurityDelete)]
        public async Task<ActionResult> Delete([FromQuery] string[] names)
        {
            if (names == null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            if (names.Any(x => !IsUserEditable(x)))
            {
                return BadRequest(new IdentityError() {Description = "It is forbidden to edit these users."});
            }

            foreach (var userName in names)
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result);
                    }
                }
            }

            return Ok(IdentityResult.Success);
        }

        /// <summary>
        /// Checks if user locked
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("users/{id}/locked")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult<UserLockedResult>> IsUserLocked([FromRoute] string id)
        {
            var result = new UserLockedResult(false);
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                result.Locked = await _userManager.IsLockedOutAsync(user);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lock user
        /// </summary>
        /// <param name="id">>User id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("users/{id}/lock")]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult<SecurityResult>> LockUser([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);
                return Ok(IdentityResultExtensions.ToSecurityResult(result));
            }

            return Ok(IdentityResultExtensions.ToSecurityResult(IdentityResult.Failed()));
        }

        /// <summary>
        /// Unlock user
        /// </summary>
        /// <param name="id">>User id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("users/{id}/unlock")]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult<SecurityResult>> UnlockUser([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MinValue);
                return Ok(IdentityResultExtensions.ToSecurityResult(result));
            }

            return Ok(IdentityResultExtensions.ToSecurityResult(IdentityResult.Failed()));
        }

        /// <summary>
        /// Verify user email
        /// </summary>
        /// <param name="userId"></param>
        [HttpPost]
        [Route("users/{userId}/sendVerificationEmail")]
        [Authorize(SecurityConstants.Permissions.SecurityVerifyEmail)]
        public async Task<ActionResult> SendVerificationEmail([FromRoute] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest(
                    IdentityResultExtensions.ToSecurityResult(
                        IdentityResult.Failed(new IdentityError {Description = "User not found"})));

            if (!IsUserEditable(user.UserName))
            {
                return BadRequest(IdentityResultExtensions.ToSecurityResult(IdentityResult
                    .Failed(new IdentityError {Description = "It is forbidden to edit this user."})));
            }

            await _commandProcessor.PublishDomainEventAsync(new UserVerifiedEmail(user.ToUser()));

            return Ok();
        }


        private bool IsUserEditable(string userName)
        {
            return _securityOptions.NonEditableUsers?.FirstOrDefault(x => x.EqualsInvariant(userName)) == null;
        }

        /// <summary>
        /// Get all registered permissions
        /// </summary>
        [HttpGet]
        [Route("permissions")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public ActionResult<Permission[]> GetAllRegisteredPermissions()
        {
            var result = _permissionsProvider.GetAllPermissions().ToArray();
            return Ok(result);
        }
    }
}