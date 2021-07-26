using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnlineStore.Modules.Identity.Api.Users.Models.Requests;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;
using OnlineStore.Modules.Identity.Application.Users.Features.ChangeUserPassword;
using OnlineStore.Modules.Identity.Application.Users.Features.GetCurrentUser;
using OnlineStore.Modules.Identity.Application.Users.Features.GetUserByEmail;
using OnlineStore.Modules.Identity.Application.Users.Features.GetUserById;
using OnlineStore.Modules.Identity.Application.Users.Features.GetUserByLogin;
using OnlineStore.Modules.Identity.Application.Users.Features.GetUserByName;
using OnlineStore.Modules.Identity.Application.Users.Features.GetUserInfo;
using OnlineStore.Modules.Identity.Application.Users.Features.RegisterNewUser;
using OnlineStore.Modules.Identity.Application.Users.Features.RequestPasswordReset;
using OnlineStore.Modules.Identity.Application.Users.Features.ResetUserPassword;
using OnlineStore.Modules.Identity.Application.Users.Features.SearchUsers;
using OnlineStore.Modules.Identity.Application.Users.Features.ValidatePasswordResetToken;
using OnlineStore.Modules.Identity.Infrastructure;

namespace OnlineStore.Modules.Identity.Api.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly AuthorizationOptions
            _securityOptions;

        private readonly ICommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IMapper _mapper;

        public UsersController(
            IOptions<AuthorizationOptions> securityOptions,
            ICommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IMapper mapper)
        {
            _securityOptions = securityOptions.Value;
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _mapper = mapper;
        }


        /// <summary>
        /// Get current user details
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("currentuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDetailDto>> GetCurrentUser()
        {
            var result = await _queryProcessor.QueryAsync(new GetCurrentUserQuery());

            return Ok(result);
        }

        /// <summary>
        /// Get user info
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("userinfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IList<Claim>>> Userinfo()
        {
            var result = await _queryProcessor.QueryAsync(new GetUserInfoQuery());

            return Ok(result);
        }

        /// <summary>
        /// SearchAsync users by keyword
        /// </summary>
        /// <param name="userSearchRequest"></param>
        [HttpPost]
        [Route("search")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserSearchResponse>> SearchUsers([FromBody] UserSearchRequest userSearchRequest)
        {
            var searchQuery = _mapper.Map<SearchUsersQuery>(userSearchRequest);
            var result = await _queryProcessor.QueryAsync(searchQuery);

            return Ok(result);
        }

        /// <summary>
        /// Get user details by user name
        /// </summary>
        /// <param name="userName"></param>
        [HttpGet]
        [Route("{userName}")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserByName([FromRoute] string userName)
        {
            var result = await _queryProcessor.QueryAsync(new GetUserByNameQuery(userName));

            return Ok(result);
        }

        /// <summary>
        /// Get user details by user ID
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [Route("id/{id}")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] Guid id)
        {
            var result = await _queryProcessor.QueryAsync(new GetUserByIdQuery(id));

            return Ok(result);
        }

        /// <summary>
        /// Get user details by user email
        /// </summary>
        /// <param name="email"></param>
        [HttpGet]
        [Route("email/{email}")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserByEmail([FromRoute] string email)
        {
            var result = await _queryProcessor.QueryAsync(new GetUserByEmailQuery(email));

            return Ok(result);
        }

        /// <summary>
        /// Get user details by external login provider
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("login/external/{loginProvider}/{providerKey}")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserByLogin([FromRoute] string loginProvider,
            [FromRoute] string providerKey)
        {
            var result = await _queryProcessor.QueryAsync(new GetUserByLoginQuery(loginProvider, providerKey));

            return Ok(result);
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("create")]
        // [Authorize(SecurityConstants.Permissions.SecurityCreate)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> CreateAsync([FromBody] RegisterNewUserRequest request)
        {
            var command = new RegisterNewUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Name,
                request.UserName,
                request.PhoneNumber,
                request.Password,
                request.Permissions.ToList(),
                request.UserType,
                request.IsAdministrator,
                request.IsActive,
                request.Roles.ToList(),
                request.LockoutEnabled,
                request.EmailConfirmed,
                request.PhotoUrl,
                request.Status);

            // await _commandProcessor.SendCommandAsync(command);

            return CreatedAtAction(nameof(GetUserById), new {id = command.Id}, command);
        }

        /// <summary>
        /// Change password for current user.
        /// </summary>
        /// <param name="changePassword">Old and new passwords.</param>
        /// <returns>Result of password change</returns>
        [HttpPost]
        [Route("currentuser/changepassword")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> ChangeCurrentUserPassword(
            [FromBody] ChangePasswordRequest changePassword)
        {
            var command = new ChangeUserPasswordCommand(User.Identity?.Name, changePassword.OldPassword,
                changePassword.NewPassword);
            await _commandProcessor.SendCommandAsync(command);

            return NoContent();
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="changePassword">Old and new passwords.</param>
        [HttpPost]
        [Route("{userName}/changepassword")]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> ChangePassword([FromRoute] string userName,
            [FromBody] ChangePasswordRequest changePassword)
        {
            var command =
                new ChangeUserPasswordCommand(userName, changePassword.OldPassword, changePassword.NewPassword);
            await _commandProcessor.SendCommandAsync(command);

            return NoContent();
        }

        /// <summary>
        /// Resets password for current user.
        /// </summary>
        /// <param name="resetPassword">Password reset information containing new password.</param>
        /// <returns>Result of password reset.</returns>
        [HttpPost]
        [Route("currentuser/resetpassword")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> ResetCurrentUserPassword(
            [FromBody] ResetPasswordRequest resetPassword)
        {
            var command = new ResetUserPasswordCommand(User.Identity?.Name, null, resetPassword.Token,
                resetPassword.NewPassword,
                resetPassword.ForcePasswordChangeOnNextSignIn);

            await _commandProcessor.SendCommandAsync(command);

            return NoContent();
        }

        /// <summary>
        /// Reset password confirmation
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="resetPassword">New password.</param>
        [HttpPost]
        [Route("{userName}/resetpassword")]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> ResetPassword([FromRoute] string userName,
            [FromBody] ResetPasswordRequest resetPassword)
        {
            var command = new ResetUserPasswordCommand(userName, null, resetPassword.Token, resetPassword.NewPassword,
                resetPassword.ForcePasswordChangeOnNextSignIn);

            await _commandProcessor.SendCommandAsync(command);

            return NoContent();
        }

        /// <summary>
        /// Reset password confirmation
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resetPassword">New password.</param>
        [HttpPost]
        [Route("id/{userId}/resetpassword")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> ResetPasswordByToken([FromRoute] string userId,
            [FromBody] ResetPasswordRequest resetPassword)
        {
            var command = new ResetUserPasswordCommand(null, userId, resetPassword.Token, resetPassword.NewPassword,
                resetPassword.ForcePasswordChangeOnNextSignIn);

            await _commandProcessor.SendCommandAsync(command);

            return NoContent();
        }

        /// <summary>
        /// Validate password reset token
        /// </summary>
        [HttpPost]
        [Route("{userId}/validatepasswordresettoken")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<bool>> ValidatePasswordResetToken(string userId,
            [FromBody] ValidatePasswordResetTokenRequest resetPasswordToken)
        {
            var command = new ValidatePasswordResetTokenCommand(userId, resetPasswordToken.Token);
            await _commandProcessor.SendCommandAsync(command);

            return NoContent();
        }

        /// <summary>
        /// Send email with instructions on how to reset user password.
        /// </summary>
        /// <remarks>
        /// Verifies provided userName and (if succeeded) sends email.
        /// </remarks>
        [HttpPost]
        [Route("{loginOrEmail}/requestpasswordreset")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> RequestPasswordReset(string loginOrEmail)
        {
            var command = new RequestPasswordResetCommand(loginOrEmail, Request.Scheme, Request.Host.Host);
            await _commandProcessor.SendCommandAsync(command);

            return NoContent();
        }

        // /// <summary>
        // /// Update user details by user ID
        // /// </summary>
        // /// <param name="user">User details.</param>
        // [HttpPut]
        // [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        // public async Task<ActionResult> Update([FromBody] UpdateUserRequest user)
        // {
        //     var command = new updatepas(loginOrEmail, Request.Scheme, Request.Host.Host);
        //     await _commandProcessor.SendCommandAsync(command);
        //     return NoContent();
        // }

        // /// <summary>
        // /// Delete users by name
        // /// </summary>
        // /// <param name="names">An array of user names.</param>
        // [HttpDelete]
        // [Authorize(SecurityConstants.Permissions.SecurityDelete)]
        // public async Task<ActionResult> Delete([FromQuery] string[] names)
        // {
        //     if (names == null)
        //     {
        //         throw new ArgumentNullException(nameof(names));
        //     }
        //
        //     if (names.Any(x => !IsUserEditable(x)))
        //     {
        //         return BadRequest(new IdentityError() {Description = "It is forbidden to edit these users."});
        //     }
        //
        //     foreach (var userName in names)
        //     {
        //         var user = await _userManager.FindByNameAsync(userName);
        //         if (user != null)
        //         {
        //             var result = await _userManager.DeleteAsync(user);
        //             if (!result.Succeeded)
        //             {
        //                 return BadRequest(result);
        //             }
        //         }
        //     }
        //
        //     return Ok(IdentityResult.Success);
        // }

        // /// <summary>
        // /// Lock user
        // /// </summary>
        // /// <param name="id">>User id</param>
        // /// <returns></returns>
        // [HttpPost]
        // [Route("{id}/lock")]
        // [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        // public async Task<ActionResult<SecurityResult>> LockUser([FromRoute] string id)
        // {
        //     var user = await _userManager.FindByIdAsync(id);
        //     if (user != null)
        //     {
        //         var result = await _userManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);
        //         return Ok(result.ToSecurityResult());
        //     }
        //
        //     return Ok(IdentityResult.Failed().ToSecurityResult());
        // }
        //
        // /// <summary>
        // /// Unlock user
        // /// </summary>
        // /// <param name="id">>User id</param>
        // /// <returns></returns>
        // [HttpPost]
        // [Route("{id}/unlock")]
        // [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        // public async Task<ActionResult<SecurityResult>> UnlockUser([FromRoute] string id)
        // {
        //     var user = await _userManager.FindByIdAsync(id);
        //     if (user != null)
        //     {
        //         await _userManager.ResetAccessFailedCountAsync(user);
        //         var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MinValue);
        //         return Ok(result.ToSecurityResult());
        //     }
        //
        //     return Ok(IdentityResult.Failed().ToSecurityResult());
        // }
        //
        // /// <summary>
        // /// Verify user email
        // /// </summary>
        // /// <param name="userId"></param>
        // [HttpPost]
        // [Route("{userId}/sendVerificationEmail")]
        // [Authorize(SecurityConstants.Permissions.SecurityVerifyEmail)]
        // public async Task<ActionResult> SendVerificationEmail([FromRoute] string userId)
        // {
        //     var user = await _userManager.FindByIdAsync(userId);
        //     if (user == null)
        //         return BadRequest(
        //             IdentityResult.Failed(new IdentityError {Description = "User not found"}).ToSecurityResult());
        //
        //     if (!IsUserEditable(user.UserName))
        //     {
        //         return BadRequest(IdentityResult
        //             .Failed(new IdentityError {Description = "It is forbidden to edit this user."}).ToSecurityResult());
        //     }
        //
        //     await _commandProcessor.PublishDomainEventAsync(new UserVerifiedEmail(user.ToUser()));
        //
        //     return Ok();
        // }
        //
        //
        // /// <summary>
        // /// Get all registered permissions
        // /// </summary>
        // [HttpGet]
        // [Route("permissions")]
        // [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        // public ActionResult<Permission[]> GetAllRegisteredPermissions()
        // {
        //     var result = _permissionsProvider.GetAllPermissions().ToArray();
        //     return Ok(result);
        // }
    }
}