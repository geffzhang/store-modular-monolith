using System;
using Common.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos;

namespace OnlineStore.Modules.Identity.Application.Users.GetUserById
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