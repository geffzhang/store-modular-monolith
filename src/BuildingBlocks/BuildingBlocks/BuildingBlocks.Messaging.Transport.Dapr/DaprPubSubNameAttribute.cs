using System;

namespace BuildingBlocks.Messaging.Transport.Dapr
{
    public class DaprPubSubNameAttribute : Attribute
    {
        public string PubSubName { get; set; } = "pubsub";
    }
}