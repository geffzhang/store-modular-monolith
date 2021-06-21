using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Messaging.Commands;
using Common.Messaging.Outbox;
using Common.Persistence;
using Common.Persistence.MSSQL;
using Common.Tests.Integration.Factory;
using Common.Tests.Integration.Fixtures;
using Common.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OnlineStore.Modules.Identity.Api;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Infrastructure;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace OnlineStore.Modules.Identity.IntegrationTests
{
    [CollectionDefinition(nameof(IdentityIntegrationTestFixture))]
    public class IdentityIntegrationTestCollection : ICollectionFixture<IdentityIntegrationTestFixture>
    {
    }

    public class IdentityIntegrationTestFixture : IAsyncLifetime
    {
        private readonly Checkpoint _checkpoint;
        private readonly IConfiguration _configuration;
        private readonly OnlineStoreApplicationFactory<Startup> _factory;
        private readonly ISqlConnectionFactory _connectionFactory;

        public IdentityIntegrationTestFixture()
        {
            _factory = new OnlineStoreApplicationFactory<Startup>();
            _configuration = _factory.Configuration; // get test configurations
            ScopeFactory = _factory.ScopeFactory;

            _checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] {"__EFMigrationsHistory"}
            };
            var scope = ScopeFactory.CreateScope();
            _connectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();

            var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
            OutboxMessagesHelper = new OutboxMessagesHelper(outbox);

            EnsureDatabase();
        }

        public OutboxMessagesHelper OutboxMessagesHelper { get; }
        public IServiceScopeFactory ScopeFactory { get; init; }

        public void SetOutput(ITestOutputHelper output)
        {
            _factory.Output = output;
        }

        public async Task ResetState()
        {
            await _checkpoint.Reset(_connectionFactory.GetOpenConnection().ConnectionString);
        }


        public async Task<TEntity> FindAsync<TEntity>(Guid id) where TEntity : class
        {
            using var scope = ScopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<IdentityDbContext>();

            return await context.FindAsync<TEntity>(id.ToString());
        }

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            using var scope = ScopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<IdentityDbContext>();

            context.Add(entity);

            await context.SaveChangesAsync();
        }

        public async Task SendAsync<T>(T request) where T : class, ICommand
        {
            using var scope = ScopeFactory.CreateScope();
            var commandProcessor = scope.ServiceProvider.GetRequiredService<ICommandProcessor>();

            await commandProcessor.SendCommandAsync(request);
        }

        public async Task<string> RunAsDefaultUserAsync()
        {
            var userName = "test@local";
            var password = "123";

            using var scope = ScopeFactory.CreateScope();

            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser {UserName = userName, Email = userName};

            await userManager.CreateAsync(user, password);

            _factory.CurrentUserId = user.Id;

            return user.Id;
        }

        private void EnsureDatabase()
        {
            using var scope = ScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<IdentityDbContext>();
            IdentityDbUpInitializer.Initialize(context?.Database.GetConnectionString());
            context?.Database.Migrate();
        }

        public Task InitializeAsync()
        {
            return ResetState();
        }

        public Task DisposeAsync()
        {
            _factory?.Dispose();
            return Task.CompletedTask;
        }
    }
}