using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Extensions.DependencyInjection;
using Common.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Shopping.API
{
    public class Startup
    {
        private readonly ISet<string> _devEnvironments = new HashSet<string> {"development", "local", "test"};
        private readonly IList<IModule> _modules;
        private readonly IList<Assembly> _assemblies;

        public Startup(IConfiguration configuration)
        {
            _assemblies = ModuleLoader.LoadAssemblies(configuration);
            _modules = ModuleLoader.LoadModules(_assemblies);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommon(_assemblies, _modules);
            foreach (var module in _modules)
            {
                module.Register(services);
            }
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation($"Modules: {string.Join(", ", _modules.Select(x => x.Name))}");
            if (_devEnvironments.Contains(env.EnvironmentName))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCommon();
            foreach (var module in _modules)
            {
                logger.LogInformation($"Configuring the middleware for: '{module.Name}'...");
                module.Use(app);
            }

            app.ValidateContracts();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", ctx => ctx.Response.WriteAsync("Online Store API"));
                endpoints.MapModuleInfo();

                foreach (var module in _modules)
                {
                    logger.LogInformation($"Configuring the endpoints for: '{module.Name}', path: '/{module.Path}'...");
                    module.ConfigureEndpoints(endpoints);
                }
            });

            _assemblies.Clear();
            _modules.Clear();
        }
    }
}