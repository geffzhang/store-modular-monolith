using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Commands;
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

        public async Task<Unit> HandleAsync(SeedIdentityCommand command, CancellationToken cancellationToken = default)
        {
            await _dataSeeder.SeedAllAsync();

            return Unit.Result;
        }
    }
}