using Common.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Features.Users.GetUserByEmail
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