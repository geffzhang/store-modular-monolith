using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Modules.Identity.Domain.UserRegistrations;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.Repositories;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.UserRegistrations
{
    public class UserRegistrationRepository : IUserRegistrationRepository
    {
        private readonly IdentityDbContext _identityDbContext;

        public UserRegistrationRepository(IdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task AddAsync(UserRegistration userRegistration)
        {
            await _identityDbContext.AddAsync(userRegistration);
        }

        public async Task<UserRegistration> GetByIdAsync(UserRegistrationId userRegistrationId)
        {
            return await _identityDbContext.Set<UserRegistration>().FirstOrDefaultAsync(x =>
                x.Id == userRegistrationId);
        }
    }
}