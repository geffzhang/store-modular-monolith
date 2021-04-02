using System.Threading.Tasks;
using OnlineStore.Modules.Users.Domain.UserRegistrations;

namespace OnlineStore.Modules.Users.Application.UserRegistrations
{
    public interface IUserRegistrationRepository
    {
        Task AddAsync(UserRegistration userRegistration);

        Task<UserRegistration> GetByIdAsync(UserRegistrationId userRegistrationId);
    }
}