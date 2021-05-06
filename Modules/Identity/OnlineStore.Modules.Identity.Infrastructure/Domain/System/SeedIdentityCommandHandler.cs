using System.Threading.Tasks;
using Common.Messaging.Commands;
using OnlineStore.Modules.Identity.Application.System;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.System
{
    public class SeedIdentityCommandHandler : ICommandHandler<SeedIdentityCommand>
    {
        private readonly DataSeeder _dataSeeder;

        public SeedIdentityCommandHandler(DataSeeder dataSeeder)
        {
            _dataSeeder = dataSeeder;
        }

        public async Task HandleAsync(SeedIdentityCommand command)
        {
            await _dataSeeder.SeedAllAsync(default);
        }
    }
}