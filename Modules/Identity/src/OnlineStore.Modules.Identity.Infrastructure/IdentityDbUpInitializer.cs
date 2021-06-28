using System.Reflection;
using Common.Messaging.Outbox;
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
                    .WithScriptsEmbeddedInAssembly(typeof(OutboxMessage).Assembly)
                    .LogToConsole()
                    .Build();
            var result = upgrader.PerformUpgrade();
        }
    }
}