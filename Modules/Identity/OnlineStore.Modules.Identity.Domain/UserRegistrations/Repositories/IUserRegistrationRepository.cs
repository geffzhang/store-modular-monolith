using System.Threading.Tasks;

namespace OnlineStore.Modules.Identity.Domain.UserRegistrations.Repositories
{
    public interface IUserRegistrationRepository
    {
        Task AddAsync(UserRegistration userRegistration);

        Task<UserRegistration> GetByIdAsync(UserRegistrationId userRegistrationId);
    }
}