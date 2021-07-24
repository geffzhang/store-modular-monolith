using System.Linq;
using AutoMapper;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;

namespace OnlineStore.Modules.Identity.Application.Users
{
    public class UsersMapping : Profile
    {
        public UsersMapping()
        {
            CreateMap<User, UserDto>()
                .ForMember(des => des.Roles, conf => conf.MapFrom(s => s.Roles.Select(r => r.Name)))
                .ForMember(des => des.Permissions, conf => conf.MapFrom(s => s.Permissions.Select(p => p.Name)));
        }
    }
}