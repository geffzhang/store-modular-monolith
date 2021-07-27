using System;
using System.Threading.Channels;
using BuildingBlocks.Core.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Transport.InMemory
{
    internal class ChannelFactory : IChannelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ChannelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        public ChannelWriter<TM> GetWriter<TM>() where TM : class, IMessage =>
            _serviceProvider.GetRequiredService<ChannelWriter<TM>>();

        public ChannelReader<TM> GetReader<TM>() where TM : class, IMessage =>
            _serviceProvider.GetRequiredService<ChannelReader<TM>>();
    }
}