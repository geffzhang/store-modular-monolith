using AutoMapper;
using OnlineStore.Modules.Identity.Api.Users.Models;
using OnlineStore.Modules.Identity.Application.Users.SearchUsers;

namespace OnlineStore.Modules.Identity.Api.Users
{
    public class UsersMapping : Profile
    {
        public UsersMapping()
        {
            CreateMap<UserSearchRequest, SearchUsersQuery>();
        }
    }
}