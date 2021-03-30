using System.Collections.Generic;

namespace IInfrastructure.Contexts
{
    public interface IIdentityContext
    {
        string Id { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
        IDictionary<string, string> Claims { get; }
    }
}