using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Common;
using Common.Messaging.Commands;
using Microsoft.Extensions.Logging;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Features.Users.Exceptions;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Domain.Users.Services;

namespace OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser
{
    public class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserEditable _userEditable;
        private readonly ILogger<RegisterNewUserCommandHandler> _logger;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserDomainToIntegrationEventMapper _userEventMapper;

        public RegisterNewUserCommandHandler(IUserRepository userRepository, IUserEditable userEditable,
            ILogger<RegisterNewUserCommandHandler> logger, ICommandProcessor commandProcessor,
            IUserDomainToIntegrationEventMapper userEventMapper)
        {
            _userRepository = userRepository;
            _userEditable = userEditable;
            _logger = logger;
            _commandProcessor = commandProcessor;
            _userEventMapper = userEventMapper;
        }

        public async Task HandleAsync(RegisterNewUserCommand command)
        {
            Guard.Against.Null(command, nameof(RegisterNewUserCommand));
            User.CheckEmailValidity(command.Email);
            User.CheckNameValidity(command.Name);

            var user = await _userRepository.FindByEmailAsync(command.Email);
            if (user is { })
            {
                _logger.LogError($"Email '{command.Email}' already in use");
                throw new EmailAlreadyInUseException(command.Email);
            }

            user = await _userRepository.FindByNameAsync(command.Name);
            if (user is { })
            {
                _logger.LogError($"UserName '{command.Name}' already in use");
                throw new UserNameAlreadyInUseException(command.Name);
            }

            user = User.Of(command.Id, command.Email, command.FirstName, command.LastName,
                command.Name, command.UserName, command.Password, command.CreatedDate, command.CreatedBy,
                command.Permissions.ToList(), command.UserType, _userEditable, command.IsAdministrator,
                command.IsActive,
                command.Roles.ToList(), command.LockoutEnabled, command.EmailConfirmed, command.PhotoUrl,
                command.Status, command.ModifiedBy, command.ModifiedDate);

            await _userRepository.AddAsync(user);
            _logger.LogInformation($"Created an account for the user with ID: '{user.Id}'.");

            // var domainEvents = user.Events.ToArray();
            // await _commandProcessor.PublishDomainEventAsync(domainEvents); // will raise our notification event
        }
    }
}