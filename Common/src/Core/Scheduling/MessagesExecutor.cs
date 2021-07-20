using System.Threading.Tasks;
using Common.Core.Messaging.Commands;
using Common.Core.Scheduling.Helpers;
using Newtonsoft.Json;

namespace Common.Core.Scheduling
{
    public class MessagesExecutor : IMessagesExecutor
    {
        private readonly ICommandProcessor _commandProcessor;
        public MessagesExecutor(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }
 
        public Task ExecuteCommand(MessageSerializedObject messageSerializedObject)
        {
            var type = messageSerializedObject.GetPayloadType();
 
            if (type != null)
            {
                dynamic req = JsonConvert.DeserializeObject(messageSerializedObject.Data, type);
 
                return _commandProcessor.SendCommandAsync(req as ICommand);
            }
 
            return null;
        }
        


    }
}