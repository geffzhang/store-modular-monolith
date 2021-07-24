namespace BuildingBlocks.Messaging.Scheduling.Quartz
{
    public class QuartzOptions
    {
        public string ConnectionString { get; set; }
        public string InstanceName { get; set; }
        public int RetryCount { get; set; }
        public int RetryIntervalMillisecond { get; set; }
    }
}