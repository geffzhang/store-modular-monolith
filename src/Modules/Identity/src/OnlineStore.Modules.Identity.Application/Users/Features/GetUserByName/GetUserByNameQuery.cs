using BuildingBlocks.Cqrs.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.Features.GetUserByName
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