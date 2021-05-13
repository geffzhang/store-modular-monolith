using System.Linq;
using AutoMapper;
using OnlineStore.Modules.Identity.Application.Users.Dtos;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Users
{
    public class UsersMapping : Profile
    {
        public UsersMapping()
        {
            CreateMap<User, UserDto>()
                .ForMember(des => des.Roles, conf => conf.MapFrom(s => s.Roles.Select(r => r.Name)))
                .ForMember(des => des.Permissions, conf => conf.MapFrom(s => s.Roles.Select(p => p.Name)));
        }
    }
}