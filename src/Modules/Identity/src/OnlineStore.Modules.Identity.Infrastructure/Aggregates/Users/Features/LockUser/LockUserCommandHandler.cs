using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Commands;
using OnlineStore.Modules.Identity.Application.Users.Features.LockUser;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.LockUser
{
    public class LockUserCommandHandler : ICommandHandler<LockUserCommand>
    {
        public Task<Unit> HandleAsync(LockUserCommand command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Unit.Result);
        }
    }
}