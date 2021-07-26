using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Commands;
using Microsoft.AspNetCore.Http;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;
using OnlineStore.Modules.Identity.Domain.Constants;

namespace OnlineStore.Modules.Identity.Application.Users.Features.UpdateUser
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateUserCommandHandler(IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(UpdateUserCommand));

            var applicationUser = await _userRepository.FindByIdAsync(command.Id.ToString());
            if (applicationUser == null)
            {
                throw new UserNotFoundException(command.UserName);
            }

            if (applicationUser.EmailConfirmed != command.EmailConfirmed
                && _httpContextAccessor.HttpContext != null
                && !_httpContextAccessor.HttpContext.User.HasGlobalPermission(SecurityConstants
                    .Permissions.SecurityVerifyEmail))
            {
                throw new UnAuthorizeUserException(command.UserName);
            }

            var user = User.Of(command.Id, command.Email, command.FirstName, command.LastName,
                command.Name, command.UserName, command.PhoneNumber, command.Password, command.UserType,
                command.IsAdministrator, command.IsActive,
                command.LockoutEnabled, command.EmailConfirmed, command.PhotoUrl,
                command.Status);

            if (!applicationUser.Email.EqualsInvariant(command.Email))
            {
                // SetEmailAsync also: sets EmailConfirmed to false and updates the SecurityStamp
                await _userRepository.SetEmailAsync(user, user.Email);
            }

            await _userRepository.UpdateAsync(user);

            return Unit.Result;
        }
    }
}