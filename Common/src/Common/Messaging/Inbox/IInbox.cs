using System;
using System.Threading.Tasks;
using Common.Messaging.Events;

namespace Common.Messaging.Inbox
{
    internal interface IInbox
    {
        bool Enabled { get; }
        Task HandleAsync(InboxMessage message, Func<Task> handler, string module);
    }
}