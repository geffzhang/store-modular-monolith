using System;

namespace Common.Contexts
{
    public interface IContext
    {
        string RequestId { get; }
        Guid CorrelationId { get; }
        string TraceId { get; }
        IIdentityContext Identity { get; }
    }
}