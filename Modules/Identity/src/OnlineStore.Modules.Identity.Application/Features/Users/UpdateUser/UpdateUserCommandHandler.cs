using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Common.Core.Extensions;
using Common.Core.Messaging.Commands;
using Microsoft.AspNetCore.Http;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Features.Users.Exceptions;
using OnlineStore.Modules.Identity.Domain;
using OnlineStore.Modules.Identity.Domain.Constants;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Domain.Users.Services;

namespace OnlineStore.Modules.Identity.Application.Features.Users.UpdateUser
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserEditable _userEditable;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateUserCommandHandler(IUserRepository userRepository, IUserEditable userEditable,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _userEditable = userEditable;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(UpdateUserCommand command)
        {
            Guard.Against.Null(command, nameof(UpdateUserCommand));

            var applicationUser = await _userRepository.FindByIdAsync(command.Id.ToString());
            if (applicationUser == null)
            {
                throw new UserNotFoundException(command.UserName);
            }

            if (_userEditable.IsUserEditable(applicationUser.UserName))
            {
                throw new UserCanNotEditException(applicationUser.UserName);
            }

            if (applicationUser.EmailConfirmed != command.EmailConfirmed
                && _httpContextAccessor.HttpContext != null
                && !_httpContextAccessor.HttpContext.User.HasGlobalPermission(SecurityConstants
                    .Permissions.SecurityVerifyEmail))
            {
                throw new UnAuthorizeUserException(command.UserName);
            }

            var user =  User.Of(command.Id, command.Email, command.FirstName, command.LastName,
                command.Name, command.UserName, command.Password, command.UserType, _userEditable,
                command.IsAdministrator, command.IsActive, 
                command.LockoutEnabled, command.EmailConfirmed, command.PhotoUrl,
                command.Status);

            if (!applicationUser.Email.EqualsInvariant(command.Email))
            {
                // SetEmailAsync also: sets EmailConfirmed to false and updates the SecurityStamp
                await _userRepository.SetEmailAsync(user, user.Email);
            }

            await _userRepository.UpdateAsync(user);
        }
    }
}