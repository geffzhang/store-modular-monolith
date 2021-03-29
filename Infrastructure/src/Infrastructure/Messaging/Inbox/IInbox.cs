using System;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Inbox
{
    internal interface IInbox
    {
        bool Enabled { get; }
        Task HandleAsync(IMessage message, Func<Task> handler, string module);
    }
}