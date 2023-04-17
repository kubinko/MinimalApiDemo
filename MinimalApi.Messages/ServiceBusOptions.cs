namespace MinimalApi.Messages
{
    public class ServiceBusOptions
    {
        public string ConnectionString { get; set; } = "";
        public Dictionary<string, QueueOptions> Queues { get; set; } = new();
        public Dictionary<string, TopicOptions> Topics { get; set; } = new();
        public Dictionary<string, SubscriptionOptions> Subscriptions { get; set; } = new();
    }

    public class TopicOptions
    {
        public string TopicName { get; set; } = "";
    }

    public class SubscriptionOptions : TopicOptions
    {
        public string SubscriptionName { get; set; } = "";
    }

    public class QueueOptions
    {
        public string QueueName { get; set; } = "";
    }
}
