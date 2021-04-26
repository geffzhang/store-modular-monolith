using System.Threading.Tasks;

namespace OnlineStore.Modules.Identity.Domain.Users
{
    public interface IUserRepository
    {
        Task<CreateUserResponse> AddAsync(User user);
    }
}