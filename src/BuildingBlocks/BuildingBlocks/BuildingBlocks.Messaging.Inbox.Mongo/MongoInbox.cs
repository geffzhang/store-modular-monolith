using System;
using System.Threading.Tasks;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging.Inbox;
using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Persistence.Mongo;
using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BuildingBlocks.Messaging.Inbox.Mongo
{
    internal sealed class MongoInbox : IInbox
    {
        private readonly string _collectionName;
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoInbox> _logger;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IMongoSessionFactory _sessionFactory;
        private readonly bool _transactionsEnabled;

        public MongoInbox(IMongoSessionFactory sessionFactory, IOptions<InboxOptions> inboxOptions, MongoOptions mongoOptions,
            IMongoDatabase database, ILogger<MongoInbox> logger,IMessageSerializer messageSerializer)
        {
            _sessionFactory = sessionFactory;
            _database = database;
            _logger = logger;
            _messageSerializer = messageSerializer;
            _transactionsEnabled = !mongoOptions.DisableTransactions;
            Enabled = inboxOptions.Value.Enabled;
            _collectionName = string.IsNullOrWhiteSpace(inboxOptions.Value.CollectionName)
                ? "inbox"
                : inboxOptions.Value.CollectionName;
        }

        public bool Enabled { get; }

        public async Task HandleAsync(InboxMessage message, Func<Task> handler, string module)
        {
            if (!Enabled)
            {
                _logger.LogWarning("Inbox is disabled, incoming messages won't be processed.");
                return;
            }

            if (message.Id == Guid.Empty)
            {
                // A synchronous request
                await handler();
                return;
            }

            var collection = _database.GetCollection<InboxMessage>($"{module}-module.{_collectionName}");
            _logger.LogTrace($"Received a message with ID: '{message.Id}' to be processed.");
            if (await collection.AsQueryable().AnyAsync(x => x.Id == message.Id))
            {
                _logger.LogWarning($"Message with ID: '{message.Id}' was already processed.");
                return;
            }

            IClientSessionHandle session = null;
            if (_transactionsEnabled)
            {
                session = await _sessionFactory.CreateAsync();
                session.StartTransaction();
            }

            try
            {
                _logger.LogTrace($"Handling a message with ID: '{message.Id}'...");
                await handler();
                await collection.InsertOneAsync(new InboxMessage
                {
                    Id = message.Id,
                    CorrelationId = message.CorrelationId,
                    Name = message.GetType().Name.Underscore(),
                    Module = message.GetModuleName(),
                    ProcessedDate = DateTime.Now
                });
                if (session is { }) await session.CommitTransactionAsync();

                _logger.LogTrace($"Handled a message with ID: '{message.Id}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error when handling a message with ID: '{message.Id}'.");
                if (session is { }) await session.AbortTransactionAsync();

                throw;
            }
            finally
            {
                session?.Dispose();
            }
        }
    }
}