namespace Common.Web.Contexts
{
    public class ExecutionContextAccessor : IExecutionContextAccessor
    {
        private readonly IExecutionContextFactory _executionContextFactory;

        public ExecutionContextAccessor(IExecutionContextFactory executionContextFactory)
        {
            _executionContextFactory = executionContextFactory;
        }

        public ExecutionContext ExecutionContext => _executionContextFactory.Create();
    }
}