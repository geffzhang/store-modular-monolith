using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnlineStore.Modules.Identity.Api.Models;
using OnlineStore.Modules.Identity.Application.Authentication.Dtos;
using OnlineStore.Modules.Identity.Application.Permissions;
using OnlineStore.Modules.Identity.Application.Permissions.Services;
using OnlineStore.Modules.Identity.Application.Search;
using OnlineStore.Modules.Identity.Application.Search.Dtos;
using OnlineStore.Modules.Identity.Domain;
using OnlineStore.Modules.Identity.Domain.Users.DomainEvents;
using OnlineStore.Modules.Identity.Domain.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;
using OnlineStore.Modules.Identity.Infrastructure.Extensions;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace OnlineStore.Modules.Identity.Api.Controllers
{
    [ApiController]
    [Route("accounts")]
    public class AccountsController : ControllerBase
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

        public AccountsController(SignInManager<ApplicationUser> signInManager,
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
        /// Sign in with user name and password
        /// </summary>
        /// <remarks>
        /// Verifies provided credentials and if succeeded returns full user details, otherwise returns 401 Unauthorized.
        /// </remarks>
        /// <param name="request">Login request.</param>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<SignInResult>> Login([FromBody] LoginRequest request)
        {
            var loginResult =
                await _signInManager.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe, true);
            if (loginResult.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                await _commandProcessor.PublishDomainEventAsync(new UserLoggedIn(user.Id));
                //Do not allow login to admin customers and rejected users
                if (await _signInManager.UserManager.IsInRoleAsync(user, SecurityConstants.SystemRoles.Customer))
                    loginResult = SignInResult.NotAllowed;
            }

            return Ok(loginResult);
        }

        /// <summary>
        /// Sign out
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("logout")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Logout()
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                await _commandProcessor.PublishDomainEventAsync(new UserLoggedOut(user.Id));
            }

            return NoContent();
        }

        /// <summary>
        /// SearchAsync roles by keyword
        /// </summary>
        /// <param name="request">SearchAsync parameters.</param>
        [HttpPost]
        [Route("roles/search")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult<RoleSearchResult>> SearchRoles([FromBody] RoleSearchCriteria request)
        {
            var result = await _roleSearchService.SearchRolesAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        /// <param name="roleName"></param>
        [HttpGet]
        [Route("roles/{roleName}")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult<ApplicationRole>> GetRole([FromRoute] string roleName)
        {
            var result = await _roleManager.FindByNameAsync(roleName);
            return Ok(result);
        }

        /// <summary>
        /// Delete roles by ID
        /// </summary>
        /// <param name="roleIds">An array of role IDs.</param>
        [HttpDelete]
        [Route("roles")]
        [Authorize(SecurityConstants.Permissions.SecurityDelete)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteRoles([FromQuery(Name = "ids")] string[] roleIds)
        {
            if (roleIds != null)
            {
                foreach (var roleId in roleIds)
                {
                    var role = await _roleManager.FindByIdAsync(roleId);
                    if (role != null)
                    {
                        await _roleManager.DeleteAsync(role);
                    }
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Update an existing role or create new
        /// </summary>
        /// <param name="role"></param>
        [HttpPut]
        [Route("roles")]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult<SecurityResult>> UpdateRole([FromBody] ApplicationRole role)
        {
            IdentityResult result;
            var roleExists = string.IsNullOrEmpty(role.Id)
                ? await _roleManager.RoleExistsAsync(role.Name)
                : await _roleManager.FindByIdAsync(role.Id) != null;
            if (!roleExists)
            {
                result = await _roleManager.CreateAsync(role);
            }
            else
            {
                result = await _roleManager.UpdateAsync(role);
            }

            return Ok(IdentityResultExtensions.ToSecurityResult(result));
        }




        [HttpPost]
        [Route("validatepassword")]
        [Authorize]
        public async Task<ActionResult<IdentityResult>> ValidatePassword([FromBody] string password)
        {
            var result = await _passwordCheckService.ValidateAsync(_userManager, null, password);
            return Ok(result);
        }


    }
}