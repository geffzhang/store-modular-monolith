using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineStore.Modules.Identity.Application.Users.Dtos;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Users.GetCurrentUser;
using OnlineStore.Modules.Identity.Application.Users.Services;
using OnlineStore.Modules.Identity.Domain.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Authentication;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, UserDetailDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserNameResolver _userNameResolver;
        private readonly UserOptionsExtended _userOptionsExtended;

        public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager,
            IUserNameResolver userNameResolver,
            IOptions<UserOptionsExtended> userOptionsExtended)
        {
            _userManager = userManager;
            _userNameResolver = userNameResolver;
            _userOptionsExtended = userOptionsExtended.Value;
        }

        public async Task<UserDetailDto> HandleAsync(GetCurrentUserQuery query)
        {
            var userName = _userNameResolver.GetCurrentUserName();
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
            {
                throw new UserNotFoundException(userName);
            }

            var result = new UserDetailDto()
            {
                Id = user.Id,
                IsAdministrator = user.IsAdministrator,
                UserName = user.UserName,
                PasswordExpired = user.PasswordExpired,
                DaysTillPasswordExpiry =
                    PasswordExpiryHelper.ContDaysTillPasswordExpiry(user, _userOptionsExtended),
                Permissions = user.Roles.SelectMany(x => x.Permissions).Select(x => x.Name).Distinct().ToArray()
            };

            return result;
        }
    }
}