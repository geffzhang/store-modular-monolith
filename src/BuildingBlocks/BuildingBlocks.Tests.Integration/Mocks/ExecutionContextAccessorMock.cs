using BuildingBlocks.Web.Contexts;

namespace BuildingBlocks.Tests.Integration.Mocks
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