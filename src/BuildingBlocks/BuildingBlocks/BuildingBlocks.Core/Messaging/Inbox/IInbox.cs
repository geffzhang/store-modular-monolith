using System;
using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging.Inbox
{
    internal interface IInbox
    {
        bool Enabled { get; }
        Task HandleAsync(InboxMessage message, Func<Task> handler, string module);
    }
}