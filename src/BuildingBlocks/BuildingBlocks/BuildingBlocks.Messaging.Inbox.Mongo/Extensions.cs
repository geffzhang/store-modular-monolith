using BuildingBlocks.Core.Messaging.Commands;
using BuildingBlocks.Core.Messaging.Inbox;
using BuildingBlocks.Core.Messaging.Outbox;
using BuildingBlocks.Messaging.Outbox.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Inbox.Mongo
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