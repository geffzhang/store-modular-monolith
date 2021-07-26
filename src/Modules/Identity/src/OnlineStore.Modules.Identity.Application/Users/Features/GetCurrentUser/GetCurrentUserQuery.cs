using BuildingBlocks.Cqrs.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.Features.GetCurrentUser
{
    public class GetCurrentUserQuery : IQuery<UserDetailDto>
    {
    }
}