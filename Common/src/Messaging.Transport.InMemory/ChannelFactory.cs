using System;
using System.Threading.Channels;
using Common.Core.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Transport.InMemory
{
    internal class ChannelFactory : IChannelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ChannelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        // public ChannelReader<MyTestMessage> Reader => _messages.Reader;
        // public ChannelWriter<MyTestMessage> Writer => _messages.Writer;

        // public ChannelReader<TMessage> GetReader<TMessage>() where TMessage : class, IMessage
        // {
        //    return _serviceProvider.GetRequiredService<ChannelReader<TMessage>>();
        // }
        //
        // public ChannelWriter<TMessage> GetWriter<TMessage>() where TMessage : class, IMessage
        // {
        //     return _serviceProvider.GetRequiredService<ChannelWriter<TMessage>>();
        // }
        // private readonly IServiceProvider _serviceProvider;
        //
        // public ChannelFactory(IServiceProvider serviceProvider)
        // {
        //     _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        // }
        //
        public ChannelWriter<TM> GetWriter<TM>() where TM : class, IMessage =>
            _serviceProvider.GetRequiredService<ChannelWriter<TM>>();

        public ChannelReader<TM> GetReader<TM>() where TM : class, IMessage =>
            _serviceProvider.GetRequiredService<ChannelReader<TM>>();
    }
}