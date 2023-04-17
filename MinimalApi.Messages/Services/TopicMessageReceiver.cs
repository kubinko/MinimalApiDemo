using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Messages;

namespace MinimalApi.Messaging.Services
{
    public abstract class TopicMessageReceiver<T> : ServiceBusMessageReceiver<T>
    {
        private readonly string _topic;
        private readonly string _subscription;

        public TopicMessageReceiver(
            IOptions<ServiceBusOptions> options,
            string optionsKey,
            ILogger<TopicMessageReceiver<T>> logger) : base(options, logger)
        {
            if (options?.Value?.Subscriptions?.TryGetValue(optionsKey, out SubscriptionOptions? topicOptions) == true)
            {
                _topic = topicOptions.TopicName;
                _subscription = topicOptions.SubscriptionName;
            }
            else
            {
                throw new ArgumentNullException(nameof(SubscriptionOptions));
            }
        }

        protected override async Task<ServiceBusProcessor> CreateProcessor(CancellationToken cancellationToken)
        {
            if (!await _serviceBusAdminClient.TopicExistsAsync(_topic, cancellationToken))
            {
                await _serviceBusAdminClient.CreateTopicAsync(_topic, cancellationToken);
            }
            if (!await _serviceBusAdminClient.SubscriptionExistsAsync(_topic, _subscription, cancellationToken))
            {
                await _serviceBusAdminClient.CreateSubscriptionAsync(_topic, _subscription, cancellationToken);
            }

            return _serviceBusClient.CreateProcessor(_topic, _subscription);
        }
    }
}
