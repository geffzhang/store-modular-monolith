using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Commands;
using Common.Messaging.Serialization;
using Common.Modules;
using Common.Persistence.Mongo;
using Common.Scheduling;
using Common.Scheduling.Helpers;
using Common.Utils;
using Common.Utils.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Common.Messaging.Scheduling.Internal.MessagesScheduler
{
    public class InternalMessageScheduler : IEnqueueMessages
    {
        private readonly IMongoSessionFactory _sessionFactory;
        private readonly MongoOptions _mongoOptions;
        private readonly IMongoDatabase _database;
        private readonly ILogger<InternalMessageScheduler> _logger;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IModuleClient _moduleClient;
        private readonly string _collectionName;
        private readonly string[] _modules;

        public InternalMessageScheduler(IMongoSessionFactory sessionFactory,
            IOptions<MongoOptions> mongoOptions,
            IOptions<InternalMessageOptions> internalMessageOptions,
            IMongoDatabase database, ILogger<InternalMessageScheduler> logger,
            IMessageSerializer messageSerializer,
            IModuleClient moduleClient, IModuleRegistry moduleRegistry)
        {
            _sessionFactory = sessionFactory;
            _mongoOptions = mongoOptions.Value;
            _database = database;
            _logger = logger;
            _messageSerializer = messageSerializer;
            _moduleClient = moduleClient;
            Enabled = internalMessageOptions.Value.Enabled;
            _modules = moduleRegistry.Modules.ToArray();
            _collectionName = string.IsNullOrWhiteSpace(internalMessageOptions.Value.CollectionName)
                ? "internal-message"
                : internalMessageOptions.Value.CollectionName;
        }

        public bool Enabled { get; set; }

        public async Task<string> Enqueue(ICommand command, string description = null)
        {
            if (!Enabled)
            {
                _logger.LogWarning("Internal-Message is disabled, outgoing messages won't be saved.");
                return null;
            }

            var internalMessages = new InternalMessage
            {
                Id = command.Id,
                CorrelationId = command.CorrelationId,
                Name = command.GetType().Name.Underscore(),
                Payload = _messageSerializer.Serialize(command),
                Type = command.GetType().AssemblyQualifiedName,
                ReceivedAt = DateTime.Now
            };

            var module = command.GetModuleName();
            await _database.GetCollection<InternalMessage>($"{module}-module.{_collectionName}")
                .InsertOneAsync(internalMessages);

            _logger.LogInformation($"Saved message to the internal-message ('{module}').");

            return command.Id.ToString();
        }

        public Task<string> Enqueue(MessageSerializedObject messageSerializedObject, string description = null)
        {
            dynamic req = _messageSerializer.Deserialize(messageSerializedObject.Data,
                messageSerializedObject.GetPayloadType());
            var command = req as ICommand;

            return Enqueue(command, description);
        }

        private class InternalMessage
        {
            public Guid Id { get; set; }
            public Guid CorrelationId { get; set; }
            public DateTime ReceivedAt { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Payload { get; set; }
        }
    }
}