using System;

namespace Common.Messaging.Transport.Dapr
{
    public class DaprPubSubNameAttribute : Attribute
    {
        public string PubSubName { get; set; } = "pubsub";
    }
}