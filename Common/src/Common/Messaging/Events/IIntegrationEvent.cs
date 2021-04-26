using System;

namespace Common.Messaging.Events
{
    //Marker
    public interface IIntegrationEvent : IEvent
    {
        DateTime OccurredOn { get; }
    }
}