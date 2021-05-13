using System.Threading.Tasks;
using Common.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos;

namespace OnlineStore.Modules.Identity.Application.Users.GetCurrentUser
{
    public class GetCurrentUserQueryHandler: IQueryHandler<GetCurrentUserQuery,UserDetailDto>
    {
        public GetCurrentUserQueryHandler(Usermana)
        {

        }
        public Task<UserDetailDto> HandleAsync(GetCurrentUserQuery query)
        {
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                return NotFound();
            }

            var result = new UserDetailResponse
            {
                Id = user.Id,
                IsAdministrator = user.IsAdministrator,
                UserName = user.UserName,
                PasswordExpired = user.PasswordExpired,
                DaysTillPasswordExpiry =
                    PasswordExpiryHelper.ContDaysTillPasswordExpiry(user, _userOptionsExtended),
                Permissions = user.Roles.SelectMany(x => x.Permissions).Select(x => x.Name).Distinct().ToArray()
            };

        }
    }
}