using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Messages;

namespace MinimalApi.Messaging.Services
{
    public abstract class QueueMessageReceiver<T> : ServiceBusMessageReceiver<T>
    {
        private readonly string _queue;

        public QueueMessageReceiver(
            IOptions<ServiceBusOptions> options,
            string optionsKey,
            ILogger<QueueMessageReceiver<T>> logger) : base(options, logger)
        {
            if (options?.Value?.Queues?.TryGetValue(optionsKey, out QueueOptions? queueOptions) == true)
            {
                _queue = queueOptions.QueueName;
            }
            else
            {
                throw new ArgumentNullException(nameof(QueueOptions));
            }
        }

        protected override async Task<ServiceBusProcessor> CreateProcessor(CancellationToken cancellationToken)
        {
            if (!await _serviceBusAdminClient.QueueExistsAsync(_queue, cancellationToken))
            {
                await _serviceBusAdminClient.CreateQueueAsync(_queue, cancellationToken);
            }

            return _serviceBusClient.CreateProcessor(_queue);
        }
    }
}
