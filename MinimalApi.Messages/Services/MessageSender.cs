using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Messages;
using System.Text.Json;

namespace MinimalApi.Messaging.Services
{
    public class MessageSender : IMessageSender
    {
        private readonly ServiceBusAdministrationClient? _serviceBusAdminClient;
        private readonly ServiceBusClient? _serviceBusClient;
        private readonly ILogger<MessageSender> _logger;

        public MessageSender(IOptions<ServiceBusOptions> options, ILogger<MessageSender> logger)
        {
            string? serviceBusConnectionString = options?.Value?.ConnectionString;
            if (!string.IsNullOrEmpty(serviceBusConnectionString))
            {
                _serviceBusAdminClient = new ServiceBusAdministrationClient(serviceBusConnectionString);
                _serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
            }

            _logger = logger;
        }

        public async Task SendMessageToQueue<T>(string queueName, T message, CancellationToken cancellationToken = default)
        {
            if (_serviceBusAdminClient == null || _serviceBusClient == null)
            {
                return;
            }

            try
            {
                if (!await _serviceBusAdminClient.QueueExistsAsync(queueName, cancellationToken))
                {
                    await _serviceBusAdminClient.CreateQueueAsync(queueName, cancellationToken);
                }

                await SendMessage(queueName, message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message [{queueName}]: {ex.Message}");
            }
        }

        public async Task SendMessageToTopic<T>(string topicName, T message, CancellationToken cancellationToken = default)
        {
            if (_serviceBusAdminClient == null || _serviceBusClient == null)
            {
                return;
            }

            try
            {
                if (!await _serviceBusAdminClient.TopicExistsAsync(topicName, cancellationToken))
                {
                    await _serviceBusAdminClient.CreateTopicAsync(topicName, cancellationToken);
                }

                await SendMessage(topicName, message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message [{topicName}]: {ex.Message}");
            }
        }

        private async Task SendMessage<T>(string queueOrTopicName, T message, CancellationToken cancellationToken)
        {
            if (_serviceBusAdminClient == null || _serviceBusClient == null)
            {
                return;
            }

            await using ServiceBusSender sender = _serviceBusClient.CreateSender(queueOrTopicName);

            string messageBody = JsonSerializer.Serialize(message);
            var busMesage = new ServiceBusMessage(messageBody);
            await sender.SendMessageAsync(busMesage, cancellationToken);

            _logger.LogInformation($"Sent message [{sender.EntityPath}]: {messageBody}");
        }
    }
}
