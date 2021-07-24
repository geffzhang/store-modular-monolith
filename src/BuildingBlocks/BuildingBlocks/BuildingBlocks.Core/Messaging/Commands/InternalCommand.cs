using System;

namespace BuildingBlocks.Core.Messaging.Commands
{
    public class InternalCommand : ICommand
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public DateTime? ProcessedDate { get; set; }
    }
}