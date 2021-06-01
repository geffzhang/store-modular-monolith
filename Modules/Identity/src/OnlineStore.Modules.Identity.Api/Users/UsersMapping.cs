using AutoMapper;
using OnlineStore.Modules.Identity.Api.Users.Models.Requests;
using OnlineStore.Modules.Identity.Application.Features.Users.SearchUsers;

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