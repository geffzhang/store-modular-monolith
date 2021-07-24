using System.Collections.Generic;

namespace BuildingBlocks.Web.Contexts
{
    public interface ICorrelationContextAccessor
    {
        /// <summary>
        /// Get CorrelationContext based on 'Correlation-Context' key in header
        /// </summary>
        CorrelationContext CorrelationContext { get; }
        
        /// <summary>
        /// Get CorrelationContext based on all keys that begin with 'x-correlation-' in header
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<string, object> GetCorrelationContextValues();
    }
}