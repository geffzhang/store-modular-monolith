using System.Threading.Tasks;
using Common.Core.Messaging.Commands;
using OnlineStore.Modules.Identity.Application.System;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.System
{
    public class SeedIdentityCommandHandler : ICommandHandler<SeedIdentityCommand>
    {
        private readonly IDataSeeder _dataSeeder;

        public SeedIdentityCommandHandler(IDataSeeder dataSeeder)
        {
            _dataSeeder = dataSeeder;
        }

        public async Task HandleAsync(SeedIdentityCommand command)
        {
            await _dataSeeder.SeedAllAsync();
        }
    }
}