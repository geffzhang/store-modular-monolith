using System;
using BuildingBlocks.Cqrs.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.Features.GetUserById
{
    public class GetUserByIdQuery : IQuery<UserDto>
    {
        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; }
    }
}