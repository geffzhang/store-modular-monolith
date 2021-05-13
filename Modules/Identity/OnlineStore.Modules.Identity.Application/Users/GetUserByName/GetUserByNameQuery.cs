using Common.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos;

namespace OnlineStore.Modules.Identity.Application.Users.GetUserByName
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