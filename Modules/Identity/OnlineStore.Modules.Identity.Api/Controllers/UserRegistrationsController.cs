using System;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Modules.Identity.Application.ConfirmUserRegistration;
using OnlineStore.Modules.Identity.Application.RegisterNewUser;
using OnlineStore.Modules.Identity.Application.Users.Dtos;

namespace OnlineStore.Modules.Identity.Api.Controllers
{
    [Route("userAccess/[controller]")]
    [ApiController]
    public class UserRegistrationsController : ControllerBase
    {
        private readonly ICommandProcessor _commandProcessor;

        public UserRegistrationsController(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        // [NoPermissionRequired]
        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> RegisterNewUser(RegisterNewUserRequest request)
        {
            // await _commandProcessor.SendCommandAsync(new RegisterNewUserCommand(
            //     request.Login,
            //     request.Password,
            //     request.Email,
            //     request.FirstName,
            //     request.LastName,
            //     request.ConfirmLink,
            //     request.Address));

            return Ok();
        }

        // [NoPermissionRequired]
        [AllowAnonymous]
        [HttpPatch("{userRegistrationId}/confirm")]
        public async Task<IActionResult> ConfirmRegistration(Guid userRegistrationId)
        {
            await _commandProcessor.SendCommandAsync(new ConfirmUserRegistrationCommand(userRegistrationId));

            return Ok();
        }
    }
}