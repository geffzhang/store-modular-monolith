using System;

namespace Common.Web.Contexts
{
    public class ExecutionContext
    {
        public string RequestId { get; init; }
        public string SpanContext { get; init; }
        public IdentityContext IdentityContext { get; init; }
        public string ResourceId { get; init; }
        public string TraceId { get; init; }
        public string ConnectionId { get; init; }
        public string Name { get; init; }
    }
}