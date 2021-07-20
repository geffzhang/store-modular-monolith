using Common.Core.Messaging.Commands;
using Common.Core.Messaging.Inbox;
using Common.Core.Messaging.Outbox;
using Common.Messaging.Outbox;
using Common.Messaging.Outbox.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Inbox.Mongo
{
    public static class Extensions
    {
        private const string SectionName = "messaging";

        public static IServiceCollection AddMongoInbox(this IServiceCollection services, IConfiguration configuration,
            string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var inboxOptions = configuration.GetSection("messaging:inbox").Get<InboxOptions>();
            services.AddOptions<InboxOptions>().Bind(configuration.GetSection("messaging:inbox")).ValidateDataAnnotations();
            
            services
                .AddTransient<IInbox, MongoInbox>()
                .AddTransient<IOutbox, MongoOutbox>();

            if (inboxOptions.Enabled)
            {
                services.TryDecorate(typeof(ICommandHandler<>), typeof(InboxCommandHandlerDecorator<>));
                // services.TryDecorate(typeof(InboxEventHandlerDecorator<>), typeof(InboxEventHandlerDecorator<>));
            }

            return services;
        }
    }
}