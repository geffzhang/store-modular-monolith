using System.Threading.Tasks;
using Common.Messaging.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users;

namespace OnlineStore.Modules.Identity.Application.System
{
    public class SeedDataCommandHandler : ICommandHandler<SeedDataCommand>
    {
        private readonly IUserRepository _userRepository;

        public SeedDataCommandHandler( IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(SeedDataCommand command)
        {
            var seeder = new DataSeeder(_userRepository);

            await seeder.SeedAllAsync(default);
        }
    }
}