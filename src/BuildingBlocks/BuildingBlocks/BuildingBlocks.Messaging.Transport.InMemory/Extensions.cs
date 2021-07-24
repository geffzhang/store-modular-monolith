using System.Linq;
using System.Reflection;
using System.Threading.Channels;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Messaging.Transport;
using BuildingBlocks.Messaging.Transport.InMemory.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Transport.InMemory
{
    public static class Extensions
    {
        public static IServiceCollection AddInMemoryMessaging(this IServiceCollection services,
            IConfiguration configuration,
            string sectionName)
        {
            var messagingOptions = configuration.GetSection(sectionName).Get<MessagingOptions>();
            services.AddOptions<MessagingOptions>().Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations();

            services.AddSingleton<IPublisher, InMemoryPublisher>()
                .AddSingleton<ITransport, InMemoryTransport>()
                .AddSingleton<InMemoryProducerDiagnostics>()
                .AddSingleton<InMemoryConsumerDiagnostics>();

            services.AddHostedService<SubscribersBackgroundService>();
            services.AddSingleton<IChannelFactory, ChannelFactory>();

            var messageTypes = MessagingHelper.GetHandledMessageTypes().ToList();
            foreach (var messageType in messageTypes)
            {
                var registerMessageMethod = RawRegisterMessageMethod.MakeGenericMethod(messageType);
                registerMessageMethod.Invoke(null, new[] {services});
            }

            return services;
        }

        private static readonly MethodInfo RawRegisterMessageMethod = typeof(Extensions)
            .GetMethod(nameof(RegisterMessage), BindingFlags.Static | BindingFlags.NonPublic);

        private static void RegisterMessage<TMessage>(IServiceCollection services) where TMessage : class, IMessage
        {
            if (services.Any(s => s.ServiceType == typeof(Channel<TMessage>)))
                return;

            services.AddSingleton(_ => Channel.CreateUnbounded<TMessage>())
                .AddSingleton(ctx =>
                {
                    var channel = ctx.GetService<Channel<TMessage>>();
                    return channel?.Reader;
                }).AddSingleton(ctx =>
                {
                    var channel = ctx.GetService<Channel<TMessage>>();
                    return channel?.Writer;
                }).AddBusSubscriber(typeof(InMemorySubscriber<TMessage>));
        }
    }
}