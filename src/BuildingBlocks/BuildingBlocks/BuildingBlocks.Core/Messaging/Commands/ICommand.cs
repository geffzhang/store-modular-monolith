using System;

namespace BuildingBlocks.Core.Messaging.Commands
{
    public interface ICommand 
    {
        public Guid Id { get; }
    }   
}