using Common.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Features.Users.GetUserByName
{
    public class GetUserByNameQuery : IQuery<UserDto>
    {
        public GetUserByNameQuery(string userName)
        {
            UserName = userName;
        }

        public string UserName { get; }
    }
}