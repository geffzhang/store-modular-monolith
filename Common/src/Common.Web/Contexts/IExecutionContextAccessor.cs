namespace Common.Web.Contexts
{
    public interface IExecutionContextAccessor
    {
        ExecutionContext ExecutionContext { get; }
    }
}