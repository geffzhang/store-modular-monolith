using AutoMapper;
using OnlineStore.Modules.Identity.Application.Users.Dtos;
using OnlineStore.Modules.Identity.Application.Users.SearchUsers;

namespace OnlineStore.Modules.Identity.Application.Users
{
    public class UserMapping: Profile
    {
        public UserMapping()
        {
            CreateMap<SearchUsersQuery, UserSearchCriteriaDto>();
            CreateMap<Usersea, SearchUsersQuery>();
        }
    }
}
