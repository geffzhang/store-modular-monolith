using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Modules
{
    public interface IModule
    {
        string Name { get; }
        string Path { get; }
        IEnumerable<string> Policies => null;
        void Register(IServiceCollection services);
        void Use(IApplicationBuilder app);
        void ConfigureEndpoints(IEndpointRouteBuilder endpoints);
    }
}