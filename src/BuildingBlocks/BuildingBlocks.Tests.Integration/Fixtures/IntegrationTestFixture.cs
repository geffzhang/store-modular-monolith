using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Messaging.Outbox;
using BuildingBlocks.Core.Messaging.Transport;
using BuildingBlocks.Cqrs.Commands;
using BuildingBlocks.Cqrs.Queries;
using BuildingBlocks.Diagnostics.Messaging.Events;
using BuildingBlocks.Diagnostics.Transports;
using BuildingBlocks.Persistence.MSSQL;
using BuildingBlocks.Tests.Integration.Constants;
using BuildingBlocks.Tests.Integration.Factory;
using BuildingBlocks.Tests.Integration.Helpers;
using BuildingBlocks.Tests.Integration.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace BuildingBlocks.Tests.Integration.Fixtures
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
        public ITransport Transport => ServiceProvider.GetRequiredService<ITransport>();

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
                var connection = OptionsHelper.GetOptions<MssqlOptions>("mssql", "appsettings.test.json")
                    .ConnectionString;
                await _checkpoint.Reset(connection);
            }
            catch
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
                catch
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

        public Task PublishAsync<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            return ExecuteScopeAsync(sp =>
            {
                var commandProcessor = sp.GetRequiredService<ICommandProcessor>();

                return commandProcessor.PublishMessageAsync(message);
            });
        }

        public Task Subscribe()
        {
            return ExecuteScopeAsync(sp =>
            {
                var transport = sp.GetRequiredService<ITransport>();
                return transport.StartAsync();
            });
        }


        public Task UnSubscribe()
        {
            return ExecuteScopeAsync(sp =>
            {
                var transport = sp.GetRequiredService<ITransport>();
                return transport.StopAsync();
            });
        }
        public Task<ObservedMessageContexts> ExecuteAndWaitForHandled<TMessageHandled>(
            Func<Task> testAction,
            TimeSpan? timeout = null) =>
            ExecuteAndWait<IMessage>(
                testAction,
                timeout);

        public Task<ObservedMessageContexts> ExecuteAndWaitForSent<TMessage>(
            Func<Task> testAction,
            TimeSpan? timeout = null) =>
            ExecuteAndWait<IMessage>(
                testAction,
                timeout);

        public Task<ObservedMessageContexts> ExecuteAndWait(
            Func<Task> testAction,
            TimeSpan? timeout = null) =>
            ExecuteAndWait<IMessage>(testAction, timeout);

        public async Task<ObservedMessageContexts> ExecuteAndWait<TMessage>(
            Func<Task> testAction,
            TimeSpan? timeout = null)
            where TMessage : IMessage
        {
            timeout ??= TimeSpan.FromSeconds(120);
            var taskCompletionSource = new TaskCompletionSource();

            var incomingMessages = new List<IMessage>();
            var outgoingMessages = new List<IMessage>();

            var obs = Observable.Empty<IMessage>();

            DiagnosticListener.AllListeners.Subscribe(delegate(DiagnosticListener listener)
            {
                // listen for 'MySampleLibrary' DiagnosticListener which inherits from abstract class DiagnosticSource
                if (listener.Name == OTelTransportOptions.InMemoryConsumerActivityName)
                {
                    //listen to specific event of listener
                    listener.Subscribe((pair) =>
                    {
                        if (pair.Key == OTelTransportOptions.Events.AfterProcessInMemoryMessage)
                        {
                            var incomingObs = listener
                                .Select(e => e.Value)
                                .Cast<IMessage>();

                            incomingObs.Subscribe(incomingMessages.Add);
                            obs = obs.Merge(incomingObs);
                        }
                    });
                }

                if (listener.Name == OTelTransportOptions.InMemoryProducerActivityName)
                {
                    listener.Subscribe((pair) =>
                    {
                        if (pair.Key == OTelTransportOptions.Events.AfterSendInMemoryMessage)
                        {
                            var outgoingObs = listener
                                .Select(e => e.Value)
                                .Cast<IMessage>();

                            outgoingObs.Subscribe(outgoingMessages.Add);
                            obs = obs.Merge(outgoingObs);
                        }
                    });
                }
            });


            var finalObs = obs.Cast<TMessage>().TakeUntil(x => x.GetType() == typeof(TMessage));
            finalObs = finalObs.Timeout(timeout.Value);

            await testAction();

            // Force the observable to complete
            await finalObs.LastOrDefaultAsync();

            return new ObservedMessageContexts(
                incomingMessages,
                outgoingMessages);
        }


        public TaskCompletionSource  EnsureReceivedMessageToConsumer<TMessage>(TMessage message = null,
            TimeSpan? timeout = null)
            where TMessage : class, IMessage
        {
            var taskCompletionSource = new TaskCompletionSource();

            DiagnosticListener.AllListeners.Subscribe(delegate(DiagnosticListener listener)
            {
                if (listener.Name == OTelTransportOptions.InMemoryConsumerActivityName)
                {
                    //listen to specific event of listener
                    listener.Subscribe((pair) =>
                    {
                        if (pair.Key == OTelTransportOptions.Events.AfterProcessInMemoryMessage)
                        {
                            taskCompletionSource.TrySetResult();
                        }
                    });
                }

                if (listener.Name == OTelTransportOptions.InMemoryProducerActivityName)
                {
                    listener.Subscribe((pair) =>
                    {
                        if (pair.Key == OTelTransportOptions.Events.AfterSendInMemoryMessage)
                        {
                            var sentMessage = pair.Value as AfterSendMessage;
                            if (sentMessage?.EventData.Id == message?.Id)
                            {
                            }
                        }
                    });
                }
            });

            Observable.Interval(timeout ?? TimeSpan.FromSeconds(120))
                .Subscribe(_ => taskCompletionSource.TrySetCanceled());

            return taskCompletionSource;
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