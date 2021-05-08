using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Search.Dtos;
using OnlineStore.Modules.Identity.Application.Users.Dtos;

namespace OnlineStore.Modules.Identity.Application.Users.Services
{
    public interface IUserSearchService
    {
        Task<UserSearchResult> SearchUsersAsync(UserSearchCriteria criteria);

    }
}