using System;

namespace Common.Web.Contexts
{
    public interface IExecutionContextAccessor
    {
        Guid UserId { get; }
        Guid CorrelationId { get; }
    }
}