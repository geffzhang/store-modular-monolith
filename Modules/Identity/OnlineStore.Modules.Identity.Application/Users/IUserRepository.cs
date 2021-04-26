using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Users.Dtos;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Users
{
    public interface IUserRepository
    {
        Task<CreateUserResponse> AddAsync(User user);
        Task<User> FindByName(string userName);
        Task<bool> CheckPassword(User user, string password);
    }
}