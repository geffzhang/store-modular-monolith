using AutoMapper;
using OnlineStore.Modules.Identity.Api.Users.Models.Requests;
using OnlineStore.Modules.Identity.Application.Users.Features.SearchUsers;

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