using System;
using Common.Core.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Features.Users.GetUserById
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