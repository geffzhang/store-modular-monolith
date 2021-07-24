namespace BuildingBlocks.Web.Contexts
{
    public interface IExecutionContextAccessor
    {
        ExecutionContext ExecutionContext { get; }
    }
}