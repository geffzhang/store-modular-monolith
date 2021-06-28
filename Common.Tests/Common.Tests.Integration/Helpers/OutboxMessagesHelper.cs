using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Domain;
using Common.Messaging.Outbox;
using Common.Web;
using Dapper;
using Newtonsoft.Json;

namespace Common.Tests.Integration.Helpers
{
    public class OutboxMessagesHelper
    {
        private readonly IOutbox _outbox;
        private IEnumerable<OutboxMessage> _messages;

        public OutboxMessagesHelper(IOutbox outbox)
        {
            _outbox = outbox;
        }

        public async Task<List<OutboxMessage>> GetOutboxMessages()
        {
            _messages = await _outbox.GetAllOutboxMessages();
            return _messages.AsList();
        }

        public T Deserialize<T>(OutboxMessage message) where T : class, IDomainEventNotification
        {
            var contractResolver = new ContractResolverWithPrivates();
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
            return JsonConvert.DeserializeObject(message.Payload, typeof(T),settings) as T;
        }

        public async Task<T> GetLastOutboxMessage<T>() where T : class, IDomainEventNotification
        {
            _messages ??= await GetOutboxMessages();
            return Deserialize<T>(_messages.Last());
        }
    }
}