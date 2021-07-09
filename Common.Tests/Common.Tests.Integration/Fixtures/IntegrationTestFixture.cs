using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Messaging;
using Common.Messaging.Commands;
using Common.Messaging.Outbox;
using Common.Messaging.Queries;
using Common.Persistence.MSSQL;
using Common.Tests.Integration.Constants;
using Common.Tests.Integration.Factory;
using Common.Tests.Integration.Helpers;
using Common.Tests.Integration.Mocks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace Common.Tests.Integration.Fixtures
{
    public class IntegrationTestFixture<TEntryPoint, TDbContext> : IAsyncLifetime
        where TEntryPoint : class
        where TDbContext : DbContext, ISqlDbContext
    {
        private readonly Checkpoint _checkpoint;

        private readonly OnlineStoreApplicationFactory<TEntryPoint> _factory;
        public IHttpClientFactory HttpClientFactory => ServiceProvider.GetRequiredService<IHttpClientFactory>();

        public OutboxMessagesHelper OutboxMessagesHelper
        {
            get
            {
                var outbox = ServiceProvider.GetRequiredService<IOutbox>();
                return new OutboxMessagesHelper(outbox);
            }
        }

        public IServiceProvider ServiceProvider => _factory.Services;
        public IConfiguration Configuration => _factory.Configuration;
        public ISqlConnectionFactory ConnectionFactory => ServiceProvider.GetRequiredService<ISqlConnectionFactory>();

        public IntegrationTestFixture()
        {
            _factory = new OnlineStoreApplicationFactory<TEntryPoint>();
            _checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] {"__EFMigrationsHistory"}
            };
        }

        public MockAuthUser CreateAdminUserMock()
        {
            var roleClaims = UsersConstants.AdminUserMock.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name));
            var otherClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, UsersConstants.AdminUserMock.UserId),
                new(ClaimTypes.Name, UsersConstants.AdminUserMock.UserName),
                new(ClaimTypes.Email, UsersConstants.AdminUserMock.UserEmail)
            };
            return _ = new MockAuthUser(roleClaims.Concat(otherClaims).ToArray());
        }

        public MockAuthUser CreateNormalUserMock()
        {
            var roleClaims = UsersConstants.NormalUserMock.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name));
            var otherClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, UsersConstants.NormalUserMock.UserId),
                new(ClaimTypes.Name, UsersConstants.NormalUserMock.UserName),
                new(ClaimTypes.Email, UsersConstants.NormalUserMock.UserEmail)
            };
            return _ = new MockAuthUser(roleClaims.Concat(otherClaims).ToArray());
        }

        public void SetOutput(ITestOutputHelper output)
        {
            _factory.OutputHelper = output;
        }

        public void RegisterTestServices(Action<IServiceCollection> services)
        {
            _factory.TestRegistrationServices = services;
        }

        private async Task ResetState()
        {
            try
            {
                var connection = OptionsHelper.GetOptions<MssqlOptions>("mssql", "appsettings.tests.json").ConnectionString;
                await _checkpoint.Reset(connection);
            }
            catch (Exception e)
            {
                // ignored
            }
        }


        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            var dbContext = ServiceProvider.GetRequiredService<TDbContext>();
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    await dbContext.BeginTransactionAsync();

                    await action(ServiceProvider);

                    await dbContext.CommitTransactionAsync();
                }
                catch (Exception _)
                {
                    dbContext?.RollbackTransaction();
                    throw;
                }
            });
        }

        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            //https://weblogs.asp.net/dixin/entity-framework-core-and-linq-to-entities-7-data-changes-and-transactions
            var dbContext = ServiceProvider.GetRequiredService<TDbContext>();
            var strategy = dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    await dbContext.BeginTransactionAsync();

                    var result = await action(ServiceProvider);

                    await dbContext.CommitTransactionAsync();

                    return result;
                }
                catch (Exception)
                {
                    dbContext?.RollbackTransaction();
                    throw;
                }
            });
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

        public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2,
            TEntity3 entity3, TEntity4 entity4)
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

        public Task<T> FindAsync<T>(object id) where T : class
        {
            return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask());
        }

        public Task SendAsync<TRequest>(TRequest request) where TRequest : class, ICommand
        {
            return ExecuteScopeAsync(sp =>
            {
                var commandProcessor = sp.GetRequiredService<ICommandProcessor>();

                return commandProcessor.SendCommandAsync(request);
            });
        }

        public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query) where TResponse : class
        {
            return ExecuteScopeAsync(sp =>
            {
                var queryProcessor = sp.GetRequiredService<IQueryProcessor>();

                return queryProcessor.QueryAsync(query);
            });
        }

        public async Task InitializeAsync()
        {
            await ResetState();
        }

        public Task DisposeAsync()
        {
            _factory?.Dispose();
            return Task.CompletedTask;
        }
    }
}