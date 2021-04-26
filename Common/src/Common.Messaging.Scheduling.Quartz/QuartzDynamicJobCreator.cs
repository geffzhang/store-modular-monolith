using System;
using System.Threading.Tasks;
using Common.BackgroundJobs;
using Common.Messaging.Scheduling;
using Common.Messaging.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Common.Scheduling.Quartz
{
    public class QuartzDynamicJobCreator<TJobInput> : IJob where TJobInput : MessageSerializedObject
    {
        private readonly ILogger<QuartzDynamicJobCreator<TJobInput>> _logger;
        private readonly IMessageSerializer _messageSerializer;
        private readonly BackgroundJobOptions _options;
        private readonly QuartzOptions _quartzOptions;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessagesExecutor _messagesExecutor;

        public QuartzDynamicJobCreator(
            IOptions<QuartzOptions> backgroundJobQuartzOptions,
            IOptions<BackgroundJobOptions> options,
            IMessagesExecutor messagesExecutor,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<QuartzDynamicJobCreator<TJobInput>> logger,
            IMessageSerializer messageSerializer)
        {
            _logger = logger;
            _messageSerializer = messageSerializer;
            _messagesExecutor = messagesExecutor;
            _serviceScopeFactory = serviceScopeFactory;
            _options = options.Value;
            _quartzOptions = backgroundJobQuartzOptions.Value;
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