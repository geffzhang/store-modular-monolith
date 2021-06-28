namespace Common.Web.Contexts
{
    public class ExecutionContextOptions
    {
        public string OperationHeaderKey { get; set; } = "x-operation";
        public string ResourceIdHeaderKey { get; set; } = "resource-id";
    }
}