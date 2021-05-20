using System;
using Common.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.Features.System
{
    public class SeedIdentityCommand : ICommand
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}