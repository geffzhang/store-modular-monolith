using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Users.Dtos;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Users
{
    public interface IUserRepository
    {
        Task<CreateUserResponse> AddAsync(User user);
        Task<UserDto> FindByNameAsync(string userName);
        Task<UserDto> FindByIdAsync(string id);
        Task<UserDto> FindByEmailAsync(string email);
        Task<UserDto> FindByLoginAsync(string loginProvider, string providerKey);
        Task<bool> CheckPassword(User user, string password);
    }
}