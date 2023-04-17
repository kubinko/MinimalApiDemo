namespace MinimalApi.Messaging.Services
{
    public interface IMessageSender
    {
        Task SendMessageToQueue<T>(string queueName, T message, CancellationToken cancellationToken);

        Task SendMessageToTopic<T>(string topicName, T message, CancellationToken cancellationToken);
    }
}
