using Common.Web.Contexts;

namespace Common.Tests.Integration.Mocks
{
    public class ExecutionContextAccessorMock : IExecutionContextAccessor
    {
        public ExecutionContextAccessorMock(ExecutionContext context)
        {
            ExecutionContext = context;
        }
        public ExecutionContext ExecutionContext { get; }
    }
}