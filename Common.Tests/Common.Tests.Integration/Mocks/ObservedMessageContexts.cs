using System.Collections.Generic;
using Common.Core.Messaging;
using Common.Messaging;

namespace Common.Tests.Integration.Mocks
{
    public class ObservedMessageContexts
    {
        public IList<IMessage> IncomingMessage { get; }
        public IList<IMessage> OutgoingMessage { get; }

        public ObservedMessageContexts(
            IList<IMessage> incomingMessage,
            IList<IMessage> outgoingMessage)
        {
            IncomingMessage = incomingMessage;
            OutgoingMessage = outgoingMessage;
        }
    }
}