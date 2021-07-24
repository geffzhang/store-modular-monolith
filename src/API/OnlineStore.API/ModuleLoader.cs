using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Core.Modules;
using Microsoft.Extensions.Configuration;

namespace OnlineStore.API
{
    public static class ModuleLoader
    {
        const string ModulePart = "OnlineStore.Modules.";
        public static IList<Assembly> LoadAssemblies(IConfiguration configuration)
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName?.Contains(ModulePart) ?? false)
                .ToList();
            var locations = assemblies.Where(x => !x.IsDynamic).Select(x => x.Location).ToArray();
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Where(x => !locations.Contains(x, StringComparer.InvariantCultureIgnoreCase))
                .ToList();

            var disabledModules = new List<string>();
            foreach (var file in files)
            {
                if (Path.GetFileName(file).Contains(ModulePart) == false)
                    continue;

                var moduleName = file.Split(ModulePart)[1].Split(".")[0].ToLowerInvariant();
                var enabled = configuration.GetValue(typeof(bool), $"{moduleName}:module:enabled");
                if (enabled is not null && (bool) enabled == false)
                {
                    disabledModules.Add(file);
                }

                assemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file)));
            }

            foreach (var disabledModule in disabledModules)
            {
                files.Remove(disabledModule);
            }

            return assemblies;
        }

        public static IList<IModule> LoadModules(IEnumerable<Assembly> assemblies)
            => assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IModule).IsAssignableFrom(x) && !x.IsInterface)
                .OrderBy(x => x.Name)
                .Select(Activator.CreateInstance)
                .Cast<IModule>()
                .ToList();
    }
}