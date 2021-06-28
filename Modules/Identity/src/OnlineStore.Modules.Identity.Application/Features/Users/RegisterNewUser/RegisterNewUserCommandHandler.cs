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
using Common.Persistence.MSSQL;

namespace OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser
{
    public class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserEditable _userEditable;
        private readonly ILogger<RegisterNewUserCommandHandler> _logger;
        private readonly ICommandProcessor _commandProcessor;
        private readonly ISqlDbContext _dbContext;
        private readonly IUserDomainToIntegrationEventMapper _userEventMapper;

        public RegisterNewUserCommandHandler(IUserRepository userRepository,
            IUserEditable userEditable,
            ILogger<RegisterNewUserCommandHandler> logger,
            ICommandProcessor commandProcessor,
            ISqlDbContext dbContext,
            IUserDomainToIntegrationEventMapper userEventMapper)
        {
            _userRepository = userRepository;
            _userEditable = userEditable;
            _logger = logger;
            _commandProcessor = commandProcessor;
            _dbContext = dbContext;
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

            user = User.Of(command.Id,
                command.Email,
                command.FirstName,
                command.LastName,
                command.Name,
                command.UserName,
                command.Password,
                command.UserType,
                _userEditable,
                command.IsAdministrator,
                command.IsActive,
                command.LockoutEnabled,
                command.EmailConfirmed,
                command.PhotoUrl,
                command.Status);

            user.AssignPermission(command.Permissions?.Select(x => Permission.Of(x, "")).ToArray());
            user.AssignRole(command.Roles?.Select(x => Role.Of(x, x)).ToArray());

            await _commandProcessor.HandleTransactionAsync(_dbContext, user.Events?.ToList(), async () =>
            {
                await _userRepository.AddAsync(user);
                _logger.LogInformation($"Created an account for the user with ID: '{user.Id}'.");
            });

            //Option1: Using our decorators for handling these operations automatically
            //Option 2: Explicit calling domain events 

            // var domainEvents = user.Events.ToArray();
            // await _commandProcessor.PublishDomainEventAsync(domainEvents); // will raise our notification event

            // var integrationEvents = _userDomainToIntegrationEventMapper.Map(notification.DomainEvent).ToArray();
            // await _commandProcessor.PublishMessageAsync(integrationEvents);
        }
    }
}