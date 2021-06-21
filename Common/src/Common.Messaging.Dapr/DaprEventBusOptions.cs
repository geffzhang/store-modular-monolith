namespace Common.Messaging.Dapr
{
    public class DaprEventBusOptions
    {
        public static string Name = "DaprEventBus";
        public string PubSubName { get; set; } = "pubsub";
    }
}