using System.Linq;
using AutoMapper;
using Common.Identity;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Roles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Role, ApplicationRole>().ConstructUsing(u => new ApplicationRole
            {
                Name = u.Name, Description = u.Description, Permissions = u.Permissions, Id = u.Name
            });

            CreateMap<ApplicationRole, Role>().ConstructUsing(au =>
                Role.Of(au.Name, au.Description, au.Permissions.ToArray()));
        }
    }
}