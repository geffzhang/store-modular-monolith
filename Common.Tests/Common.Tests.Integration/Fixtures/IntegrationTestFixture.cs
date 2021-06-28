using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Messaging.Commands;
using Common.Messaging.Outbox;
using Common.Persistence;
using Common.Persistence.MSSQL;
using Common.Tests.Integration.Factory;
using Common.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;
using Respawn;
using Xunit;

namespace Common.Tests.Integration.Fixtures
{
    public class IntegrationTestFixture<TEntryPoint, TDbContext> : IAsyncLifetime
        where TEntryPoint : class
        where TDbContext : DbContext, ISqlDbContext
    {
        private readonly Checkpoint _checkpoint;
        private readonly IConfiguration _configuration;
        private readonly OnlineStoreApplicationFactory<TEntryPoint> _factory;
        private string _currentUserId;
        private readonly ISqlConnectionFactory _connectionFactory;

        public IntegrationTestFixture()
        {
            _factory = new OnlineStoreApplicationFactory<TEntryPoint>();
            _configuration = _factory.Services.GetRequiredService<IConfiguration>(); // get test configurations

            _factory = new OnlineStoreApplicationFactory<TEntryPoint>();
            _configuration = _factory.Configuration; // get test configurations
            ScopeFactory = _factory.ScopeFactory;

            _checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] {"__EFMigrationsHistory"}
            };
            var scope = ScopeFactory.CreateScope();
            _connectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
            HttpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
            OutboxMessagesHelper = new OutboxMessagesHelper(outbox);

            EnsureDatabase();
            
            _checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] {"__EFMigrationsHistory"}
            };

            EnsureDatabase();
        }
        
        public IHttpClientFactory HttpClientFactory { get; init; }
        public OutboxMessagesHelper OutboxMessagesHelper { get; init; }
        public IServiceScopeFactory ScopeFactory { get; init; }

        private void EnsureDatabase()
        {
            using var scope = ScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<TDbContext>();

            DbUpInitializer.Initialize(context?.Database.GetConnectionString());
            context?.Database.Migrate();
        }

        public async Task ResetState()
        {
            await _checkpoint.Reset(_configuration.GetConnectionString("OnlineStore"));
            _currentUserId = null;
        }

        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = ScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

            try
            {
                await dbContext.BeginTransactionAsync();

                await action(scope.ServiceProvider);

                await dbContext.CommitTransactionAsync();
            }
            catch (Exception)
            {
                dbContext?.RollbackTransaction();
                throw;
            }
        }

        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope = ScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            
            try
            {
            
                await dbContext.BeginTransactionAsync();

                var result = await action(scope.ServiceProvider);

                await dbContext.CommitTransactionAsync();

                return result;
            }
            catch (Exception)
            {
                dbContext?.RollbackTransaction();
                throw;
            }
        }

        public Task ExecuteDbContextAsync(Func<TDbContext, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>()));

        public Task ExecuteDbContextAsync(Func<TDbContext, ValueTask> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>()).AsTask());

        public Task ExecuteDbContextAsync(Func<TDbContext, ICommandProcessor, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>(), sp.GetService<ICommandProcessor>()));

        public Task<T> ExecuteDbContextAsync<T>(Func<TDbContext, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>()));

        public Task<T> ExecuteDbContextAsync<T>(Func<TDbContext, ValueTask<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>()).AsTask());

        public Task<T> ExecuteDbContextAsync<T>(Func<TDbContext, ICommandProcessor, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>(), sp.GetService<ICommandProcessor>()));

        public Task InsertAsync<T>(params T[] entities) where T : class
        {
            return ExecuteDbContextAsync(db =>
            {
                foreach (var entity in entities)
                {
                    db.Set<T>().Add(entity);
                }

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity, TEntity2>(TEntity entity, TEntity2 entity2)
            where TEntity : class
            where TEntity2 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity, TEntity2, TEntity3>(TEntity entity, TEntity2 entity2, TEntity3 entity3)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2, TEntity3 entity3, TEntity4 entity4)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class
            where TEntity4 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);
                db.Set<TEntity4>().Add(entity4);

                return db.SaveChangesAsync();
            });
        }

        public Task<T> FindAsync<T>(int id) where T : class
        {
            return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask());
        }

        public Task SendAsync(ICommand request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var commandProcessor = sp.GetRequiredService<ICommandProcessor>();

                return commandProcessor.SendCommandAsync(request);
            });
        }

        public async Task<string> RunAsDefaultUserAsync()
        {
            var userName = "test@local";
            var password = "Testing1234!";

            using var scope = ScopeFactory.CreateScope();

            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser {UserName = userName, Email = userName};

            var result = await userManager.CreateAsync(user, password);

            _currentUserId = user.Id;

            return _currentUserId;
        }


        public Task InitializeAsync()
            => ResetState();

        public Task DisposeAsync()
        {
            _factory?.Dispose();
            return Task.CompletedTask;
        }
    }
}