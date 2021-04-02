using Common.Extensions.DependencyInjection;
using Common.Messaging.Commands;
using Common.Messaging.Outbox;
using Common.Messaging.Outbox.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Inbox.Mongo
{
    internal static class Extensions
    {
        private const string SectionName = "messaging";

        public static IServiceCollection AddMongoInbox(this IServiceCollection services,
            string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var inboxOptions = services.GetOptions<InboxOptions>($"{sectionName}:inbox");
            services
                .AddSingleton(inboxOptions)
                .AddTransient<IInbox, MongoInbox>()
                .AddTransient<IOutbox, MongoOutbox>();

            if (inboxOptions.Enabled)
            {
                services.TryDecorate(typeof(ICommandHandler<>), typeof(InboxCommandHandlerDecorator<>));
                services.TryDecorate(typeof(InboxEventHandlerDecorator<>), typeof(InboxEventHandlerDecorator<>));
            }

            return services;
        }
    }
}