using Common.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos;

namespace OnlineStore.Modules.Identity.Application.Users.GetUserByEmail
{
    public class GetUserByEmailQuery : IQuery<UserDto>
    {
        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }
}