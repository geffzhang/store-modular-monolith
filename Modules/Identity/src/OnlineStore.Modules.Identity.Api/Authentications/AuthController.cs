using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Modules.Identity.Api.Authentications.Models;

namespace OnlineStore.Modules.Identity.Api.Authentications
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        // // POST api/auth/login
        // [HttpPost("login")]
        // public async Task<ActionResult> Login([FromBody] LoginRequest request)
        // {
        //     if (!ModelState.IsValid) { return BadRequest(ModelState); }
        //     await _loginUseCase.Handle(new LoginRequest(request.UserName, request.Password, Request.HttpContext.Connection.RemoteIpAddress?.ToString()), _loginPresenter);
        //     return _loginPresenter.ContentResult;
        // }
        //
        // // POST api/auth/refreshtoken
        // [HttpPost("refreshtoken")]
        // public async Task<ActionResult> RefreshToken([FromBody] ExchangeRefreshTokenRequest request)
        // {
        //     if (!ModelState.IsValid) { return BadRequest(ModelState);}
        //     await _exchangeRefreshTokenUseCase.Handle(new ExchangeRefreshTokenRequest(request.AccessToken, request.RefreshToken, _authSettings.SecretKey), _exchangeRefreshTokenPresenter);
        //     return _exchangeRefreshTokenPresenter.ContentResult;
        // }
    }
}