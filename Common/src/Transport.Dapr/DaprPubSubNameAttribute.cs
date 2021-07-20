using System;

namespace Common.Transport.Dapr
{
    public class DaprPubSubNameAttribute : Attribute
    {
        public string PubSubName { get; set; } = "pubsub";
    }
}