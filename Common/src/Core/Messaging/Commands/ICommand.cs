using System;

namespace Common.Core.Messaging.Commands
{
    public interface ICommand 
    {
        public Guid Id { get; set; }
    }   
}