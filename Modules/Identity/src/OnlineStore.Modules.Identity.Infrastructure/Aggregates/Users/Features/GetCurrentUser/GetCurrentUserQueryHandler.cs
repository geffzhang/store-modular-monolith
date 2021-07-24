using System.Linq;
using System.Threading.Tasks;
using Common.Core.Messaging.Queries;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Users.GetCurrentUser;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;
using OnlineStore.Modules.Identity.Infrastructure.Authentication.Login;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, UserDetailDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserNameResolver _userNameResolver;

        public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager,
            IUserNameResolver userNameResolver)
        {
            _userManager = userManager;
            _userNameResolver = userNameResolver;
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
                DaysTillPasswordExpiry = PasswordExpiryHelper.DaysTillPasswordExpiry(user),
                Permissions = user.Roles.SelectMany(x => x.Permissions).Select(x => x.Name).Distinct().ToArray()
            };

            return result;
        }
    }
}