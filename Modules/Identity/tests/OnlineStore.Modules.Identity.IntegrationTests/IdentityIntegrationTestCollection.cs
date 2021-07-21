using Common.Tests.Integration.Fixtures;
using OnlineStore.API;
using OnlineStore.Modules.Identity.Api;
using OnlineStore.Modules.Identity.Infrastructure;
using Xunit;

namespace OnlineStore.Modules.Identity.IntegrationTests
{
    [CollectionDefinition(nameof(IntegrationTestFixture<Startup, IdentityDbContext>))]
    public class IdentityIntegrationTestCollection : ICollectionFixture<IntegrationTestFixture<Startup, IdentityDbContext>>
    {
    }
}