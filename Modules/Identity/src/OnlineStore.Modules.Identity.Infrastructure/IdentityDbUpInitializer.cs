using System.Reflection;
using DbUp;

namespace OnlineStore.Modules.Identity.Infrastructure
{
    public static class IdentityDbUpInitializer
    {
        public static void Initialize(string connection)
        {
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connection)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();
            var result = upgrader.PerformUpgrade();
        }
    }
}