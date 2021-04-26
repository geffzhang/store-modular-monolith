using System;
using System.Threading.Tasks;
using Common.Messaging.Serialization;
using Common.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Common.Messaging.Scheduling.Quartz
{
    public class QuartzDynamicJobCreator<TJobInput> : IJob where TJobInput : MessageSerializedObject
    {
        private readonly ILogger<QuartzDynamicJobCreator<TJobInput>> _logger;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IMessagesExecutor _messagesExecutor;

        public QuartzDynamicJobCreator(
            IOptions<QuartzOptions> backgroundJobQuartzOptions,
            IMessagesExecutor messagesExecutor,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<QuartzDynamicJobCreator<TJobInput>> logger,
            IMessageSerializer messageSerializer)
        {
            _logger = logger;
            _messageSerializer = messageSerializer;
            _messagesExecutor = messagesExecutor;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var args = _messageSerializer.Deserialize<TJobInput>(
                context.JobDetail.JobDataMap.GetString(nameof(TJobInput)));
            try
            {
                await _messagesExecutor.ExecuteCommand(args);
            }
            catch (Exception exception)
            {
                var jobExecutionException = new JobExecutionException(exception);

                throw jobExecutionException;
            }
        }
    }
}