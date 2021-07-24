using System.Reflection;
using DbUp;

namespace BuildingBlocks.Persistence.MSSQL
{
    public static class DbUpInitializer
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