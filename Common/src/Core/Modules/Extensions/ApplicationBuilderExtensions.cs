using Common.Core.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Modules.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IModuleSubscriber UseModuleRequests(this IApplicationBuilder app)
            => app.ApplicationServices.GetRequiredService<IModuleSubscriber>();

        public static IContractRegistry UseContracts(this IApplicationBuilder app)
            => app.ApplicationServices.GetRequiredService<IContractRegistry>();

    }
}