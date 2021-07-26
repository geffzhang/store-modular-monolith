using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Authentication.Jwt;
using BuildingBlocks.Core;
using BuildingBlocks.Cqrs.Commands;
using Microsoft.Extensions.Logging;
using OnlineStore.Modules.Identity.Application.Authentication.Services;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using NotImplementedException = System.NotImplementedException;

namespace OnlineStore.Modules.Identity.Application.Authentication.Features.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, JsonWebToken>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommandProcessor _commandProcessor;
        private readonly JwtHandler _jwtHandler;
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly ITokenStorageService _tokenStorageService;

        public LoginCommandHandler(IUserRepository userRepository,
            ICommandProcessor commandProcessor,
            JwtHandler jwtHandler,
            ITokenStorageService tokenStorageService,
            ILogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository;
            _commandProcessor = commandProcessor;
            _jwtHandler = jwtHandler;
            _tokenStorageService = tokenStorageService;
            _logger = logger;
        }

        public async Task<JsonWebToken> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByNameOrEmailAsync(command.UserNameOrEmail);

            if (user == null)
            {
                throw new UserNotFoundException(command.UserNameOrEmail);
            }

            var passwordIsValid = await _userRepository.CheckPassword(user, command.Password);
            if (passwordIsValid == false)
            {
                throw new LoginFailedException(command.UserNameOrEmail);
            }

            if (!user.IsActive)
            {
                throw new UserInActiveException(command.UserNameOrEmail);
            }

            if (user.EmailConfirmed == false)
            {
                throw new EmailNotConfirmedException(user.Email);
            }

            var allClaims = await _userRepository.GetClaimsAsync(user);

            var jsonWebToken = _jwtHandler.CreateToken(user.UserName, user.Email, user.Id.Id.ToString(),
                user.EmailConfirmed, user.FirstName, user.LastName, user.PhoneNumber,
                allClaims.UserClaims, allClaims.Roles, allClaims.PermissionClaims);

            user.AddRefreshToken(jsonWebToken.RefreshToken);
            await _userRepository.UpdateAsync(user);
            await _tokenStorageService.SetAsync(command.Id, jsonWebToken);
            _logger.LogInformation($"User with ID: {user.Id} has been authenticated.");

            await _commandProcessor.PublishMessageAsync(new LoggedInIntegrationEvent(user.UserName, user.Id,
                jsonWebToken));

            // we can don't return value from command and get token from a short term session in our request with `TokenStorageService`
            return jsonWebToken;
        }
    }
}