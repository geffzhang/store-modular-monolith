using System;

namespace Common.Messaging.Dapr
{
    public class DaprPubSubNameAttribute : Attribute
    {
        public string PubSubName { get; set; } = "pubsub";
    }
}