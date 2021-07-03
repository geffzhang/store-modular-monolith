namespace Common.Web.Contexts
{
    public class CorrelationContextOptions
    {
        
        public string CorrelationIdHeaderKey { get; set; } = "x-correlation-id";
        public string CorrelationContextHeaderKey { get; set; } = "correlation-context";
    }
}