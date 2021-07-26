using System;
using BuildingBlocks.Cqrs.Commands;

namespace OnlineStore.Modules.Identity.Application.System
{
    public class SeedIdentityCommand : ICommand
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}