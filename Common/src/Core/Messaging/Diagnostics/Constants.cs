namespace Common.Core.Messaging.Diagnostics
{
    public static class Constants
    {
        public static class Activities
        {
            public const string InMemoryProducerActivityName = "Diagnostics.InMemoryOutboundMessage";
            public const string InMemoryConsumerActivityName = "Diagnostics.InMemoryInboundMessage";
        }
        public static class Events
        {
            public const string AfterProcessInMemoryMessage = Activities.InMemoryConsumerActivityName + ".Stop";
            public const string BeforeProcessInMemoryMessage = Activities.InMemoryConsumerActivityName + ".Start";
            public const string BeforeSendInMemoryMessage = Activities.InMemoryProducerActivityName + ".Start";
            public const string AfterSendInMemoryMessage = Activities.InMemoryProducerActivityName + ".Stop";
        }

        public const string CorrelationContextHeaderName = "Correlation-Context";
        public const string TraceParentHeaderName = "traceparent";
        public const string RequestIdHeaderName = "request-id";
        public const string TraceStateHeaderName = "tracestate";
    }
}