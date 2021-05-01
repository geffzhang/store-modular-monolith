using System;
using Common.Messaging.Commands;

namespace OnlineStore.Modules.Identity.Application.System
{
    public class SeedDataCommand : ICommand
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}