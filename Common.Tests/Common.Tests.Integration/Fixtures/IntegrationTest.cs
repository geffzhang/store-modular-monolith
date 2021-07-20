using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Messaging;
using Common.Core.Messaging.Commands;
using Common.Core.Messaging.Outbox;
using Common.Core.Messaging.Queries;
using Common.Messaging;
using Common.Messaging.Outbox;
using Common.Persistence.MSSQL;
using Common.Tests.Integration.Constants;
using Common.Tests.Integration.Factory;
using Common.Tests.Integration.Helpers;
using Common.Tests.Integration.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace Common.Tests.Integration.Fixtures
{
    public abstract class IntegrationTest<TEntryPoint, TDbContext> :
        IClassFixture<OnlineStoreApplicationFactory<TEntryPoint>>
        where TEntryPoint : class
        where TDbContext : DbContext, ISqlDbContext
    {
        private readonly Checkpoint _checkpoint;

        protected OutboxMessagesHelper OutboxMessagesHelper { get; }
        protected WebApplicationFactory<TEntryPoint> Factory { get; set; }
        protected IServiceProvider ServiceProvider => Factory.Services;
        protected IConfiguration Configuration => ServiceProvider.GetService<IConfiguration>();
        protected ISqlConnectionFactory ConnectionFactory { get; }
        protected HttpClient HttpClient { get; }
        protected IHttpClientFactory HttpClientFactory { get; }


        protected IntegrationTest(OnlineStoreApplicationFactory<TEntryPoint> fixture, ITestOutputHelper outputHelper,
            string environment = "tests")
        {
            //https://adamstorr.azurewebsites.net/blog/integration-testing-with-aspnetcore-3-1-remove-the-boiler-plate
            //a way to swap out dependencies or we can handle this swap easier in our custom WebApplicationFactory
            Factory = fixture.WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(l =>
                {
                    if (outputHelper is not null)
                    {
                        l.ClearProviders();
                        l.AddXUnit(outputHelper);
                    }
                });

                builder.UseEnvironment(environment);
                builder.ConfigureTestServices(ConfigureTestServices);
            });
            _checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] {"__EFMigrationsHistory"}
            };
            HttpClient = Factory.CreateClient();
            ConnectionFactory = ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
            HttpClientFactory = ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var outbox = ServiceProvider.GetRequiredService<IOutbox>();
            OutboxMessagesHelper = new OutboxMessagesHelper(outbox);
            ResetState().GetAwaiter().GetResult();
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


        protected virtual void ConfigureTestServices(IServiceCollection services)
        {
        }
        public async Task ResetState()
        {
            await _checkpoint.Reset(ConnectionFactory.GetConnectionString());
        }

        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            var dbContext = ServiceProvider.GetRequiredService<TDbContext>();

            try
            {
                await dbContext.BeginTransactionAsync();

                await action(ServiceProvider);

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
            var dbContext = ServiceProvider.GetRequiredService<TDbContext>();

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

        public Task<T> FindAsync<T>(Guid id) where T : class
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
        public Task QueryAsync<TResponse>(IQuery<TResponse> query) where TResponse : class, IQuery<TResponse>
        {
            return ExecuteScopeAsync(sp =>
            {
                var queryProcessor = sp.GetRequiredService<IQueryProcessor>();

                return queryProcessor.QueryAsync(query);
            });
        }

    }
}