using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs.Queries;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Authentication.Login;

namespace OnlineStore.Modules.Identity.Application.Users.Features.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, UserDetailDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserNameResolver _userNameResolver;

        public GetCurrentUserQueryHandler(IUserRepository userRepository,
            IUserNameResolver userNameResolver)
        {
            _userRepository = userRepository;
            _userNameResolver = userNameResolver;
        }

        public async Task<UserDetailDto> HandleAsync(GetCurrentUserQuery query,
            CancellationToken cancellationToken = default)
        {
            var userName = _userNameResolver.GetCurrentUserName();
            var user = await _userRepository.FindByNameAsync(userName);
            if (user is null)
            {
                throw new UserNotFoundException(userName);
            }

            var result = new UserDetailDto()
            {
                Id = user.Id.Id.ToString(),
                IsAdministrator = user.IsAdministrator,
                UserName = user.UserName,
                PasswordExpired = user.PasswordExpired,
                DaysTillPasswordExpiry = PasswordExpiryHelper.DaysTillPasswordExpiry(user),
                Permissions = user.Roles.SelectMany(x => x.Permissions).Select(x => x.Name).Distinct().ToArray()
            };

            return result;
        }
    }
}