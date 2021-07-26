using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Authentication.Jwt;
using BuildingBlocks.Core;
using BuildingBlocks.Cqrs.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using OnlineStore.Modules.Identity.Application.Authentication.IntegrationEvents;
using OnlineStore.Modules.Identity.Application.Authentication.Services;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Specs;
using JsonWebToken = BuildingBlocks.Authentication.Jwt.JsonWebToken;

namespace OnlineStore.Modules.Identity.Application.Authentication.Features.RefreshToken
{
    public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, JsonWebToken>
    {
        private readonly IJwtTokenValidator _tokenValidator;
        private readonly IUserRepository _userRepository;
        private readonly JwtHandler _jwtHandler;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;
        private readonly ICommandProcessor _commandProcessor;
        private readonly ITokenStorageService _tokenStorageService;

        public RefreshTokenCommandHandler(IJwtTokenValidator tokenValidator, IUserRepository userRepository,
            JwtHandler jwtHandler, ITokenStorageService tokenStorageService, ILogger<RefreshTokenCommandHandler> logger,
            ICommandProcessor commandProcessor)
        {
            _tokenValidator = tokenValidator;
            _userRepository = userRepository;
            _jwtHandler = jwtHandler;
            _logger = logger;
            _commandProcessor = commandProcessor;
            _tokenStorageService = tokenStorageService;
        }

        public async Task<JsonWebToken> HandleAsync(RefreshTokenCommand command,
            CancellationToken cancellationToken = default)
        {
            var cp = _tokenValidator.GetPrincipalFromToken(command.AccessToken);

            // invalid token/signing key was passed and we can't extract user claims
            if (cp != null)
            {
                var userName = cp.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var user = await _userRepository.FindOneAsync(new UserWithAccessTokenSpecification(userName));
                if (user is null)
                    throw new UserNotFoundException();

                if (user.HasValidRefreshToken(command.RefreshToken))
                {
                    var claims = await _userRepository.GetClaimsAsync(user);
                    var jsonWebToken = _jwtHandler.CreateToken(user.UserName, user.Email, user.Id.Id.ToString(),
                        user.EmailConfirmed, user.FirstName, user.LastName, user.PhoneNumber, claims.UserClaims,
                        claims.Roles, claims.PermissionClaims);
                    var refreshToken = _jwtHandler.GenerateRefreshToken(user.Id.ToString());
                    user.RemoveRefreshToken(command.RefreshToken); // delete the token we've exchanged
                    user.AddRefreshToken(refreshToken); // add the new one
                    _userRepository.Update(user);
                    await _tokenStorageService.SetAsync(command.Id, jsonWebToken);
                    _logger.LogInformation($"Token refreshed for User with ID: {user.Id}.");

                    await _commandProcessor.PublishMessageAsync(new RefreshTokenIntegrationEvent(user.UserName, user.Id,
                        jsonWebToken));

                    return jsonWebToken;
                }
            }

            return null!;
        }
    }
}