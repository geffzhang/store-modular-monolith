using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using BuildingBlocks.Cqrs.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Users.Features.GetUserInfo;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;
using OpenIddict.Abstractions;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.GetUserInfo
{
    public class GetUserInfoQueryHandler : IQueryHandler<GetUserInfoQuery, IList<Claim>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserInfoQueryHandler(UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IList<Claim>> HandleAsync(GetUserInfoQuery query,CancellationToken cancellationToken = default)
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;
            if (currentUser == null)
            {
                throw new CurrentUserIsEmptyException();
            }

            var user = await _userManager.GetUserAsync(currentUser);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var claims = new List<Claim>
            {
                new(OpenIdConnectConstants.Claims.Subject, await _userManager.GetUserIdAsync(user))
            };

            if (currentUser.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIdConnectConstants.Scopes.Email))
            {
                claims.Add(new(OpenIdConnectConstants.Claims.Email, await _userManager.GetEmailAsync(user)));
                claims.Add(new Claim(OpenIdConnectConstants.Claims.EmailVerified,
                    (await _userManager.IsEmailConfirmedAsync(user)).ToString()));
            }

            if (currentUser.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIdConnectConstants.Scopes.Phone))
            {
                claims.Add(new(OpenIdConnectConstants.Claims.PhoneNumber, await _userManager
                    .GetPhoneNumberAsync(user)));
                claims.Add(new(OpenIdConnectConstants.Claims.PhoneNumberVerified, (await _userManager
                    .IsPhoneNumberConfirmedAsync(user)).ToString()));
            }

            if (currentUser.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIddictConstants.Scopes.Roles))
            {
                claims.Add(new Claim("roles", JArray.FromObject(await _userManager.GetRolesAsync(user)).ToString()));
            }

            return claims;
        }
    }
}