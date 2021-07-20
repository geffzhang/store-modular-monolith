using System.Threading.Tasks;
using Common.Core.Messaging.Commands;
using OnlineStore.Modules.Identity.Application.Features.System;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.System
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