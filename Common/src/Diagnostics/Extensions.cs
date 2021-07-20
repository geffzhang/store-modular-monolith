using System;
using System.Diagnostics;
using Common.Diagnostics.Transports;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Diagnostics
{
    public static class Extensions
    {
        public static IServiceCollection AddInMemoryMessagingTelemetry(this IServiceCollection services)
        {
            DiagnosticListener.AllListeners.Subscribe(listener =>
            {
                if (listener.Name == InMemoryTransportListener.InBoundName || listener.Name == InMemoryTransportListener.OutBoundName)
                {
                    listener.SubscribeWithAdapter(new InMemoryTransportListener());
                }
            });

            return services;
        }
    }
}