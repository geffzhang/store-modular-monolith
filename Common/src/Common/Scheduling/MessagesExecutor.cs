using System.ComponentModel;
using System.Threading.Tasks;
using Common.Messaging.Commands;
using Common.Messaging.Scheduling.Helpers;
using Newtonsoft.Json;

namespace Common.Messaging.Scheduling
{
    public class MessagesExecutor : IMessagesExecutor
    {
        private readonly ICommandProcessor _commandProcessor;
        public MessagesExecutor(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }
 
        [DisplayName("Processing command {0}")]
        public Task ExecuteCommand(MessageSerializedObject messageSerializedObject)
        {
            var type = messageSerializedObject.GetPayloadType();
 
            if (type != null)
            {
                dynamic req = JsonConvert.DeserializeObject(messageSerializedObject.Data, type);
 
                return this._commandProcessor.SendCommandAsync(req as ICommand);
            }
 
            return null;
        }
        


    }
}