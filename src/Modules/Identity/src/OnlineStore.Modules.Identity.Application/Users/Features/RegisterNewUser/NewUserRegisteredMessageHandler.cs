﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Persistence.MSSQL;
using Microsoft.Extensions.Logging;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;

namespace OnlineStore.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class RegisterNewUserMessageHandler : IMessageHandler<RegisterNewUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RegisterNewUserCommandHandler> _logger;
        private readonly ICommandProcessor _commandProcessor;
        private readonly ISqlDbContext _dbContext;
        private readonly IUserDomainEventsToIntegrationEventsMapper _userEventMapper;

        public RegisterNewUserMessageHandler(IUserRepository userRepository,
            ILogger<RegisterNewUserCommandHandler> logger,
            ICommandProcessor commandProcessor,
            ISqlDbContext dbContext,
            IUserDomainEventsToIntegrationEventsMapper userEventMapper)
        {
            _userRepository = userRepository;
            _logger = logger;
            _commandProcessor = commandProcessor;
            _dbContext = dbContext;
            _userEventMapper = userEventMapper;
        }

        public async Task HandleAsync(RegisterNewUserCommand command, IMessageContext context,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(RegisterNewUserCommand));
            User.CheckEmailValidity(command.Email);
            User.CheckNameValidity(command.Name);

            var user = await _userRepository.FindByEmailAsync(command.Email);
            if (user is { })
            {
                _logger.LogError($"Email '{command.Email}' already in use");
                throw new EmailAlreadyInUsedException(command.Email);
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
                command.PhoneNumber,
                command.Password,
                command.UserType,
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