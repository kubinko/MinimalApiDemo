using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Messaging.Services;

namespace MinimalApi.Common
{
    public static class BuilderExtensions
    {
        public static void AddAzureAppConfigurationWithKeyVault(this IConfigurationBuilder config, string? connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                config.AddAzureAppConfiguration(options =>
                    options.Connect(connectionString)
                        .ConfigureKeyVault(kv =>
                        {
                            kv.SetCredential(new DefaultAzureCredential());
                        }));
            }
        }

        public static IServiceCollection AddTopicReceiver<TMessage, TReceiver>(this IServiceCollection services)
            where TReceiver : TopicMessageReceiver<TMessage>
            => services
                .AddSingleton<TopicMessageReceiver<TMessage>, TReceiver>()
                .AddHostedService(services => services.GetService<TopicMessageReceiver<TMessage>>()!);

        public static IServiceCollection AddQueueReceiver<TMessage, TReceiver>(this IServiceCollection services)
            where TReceiver : QueueMessageReceiver<TMessage>
            => services
                .AddSingleton<QueueMessageReceiver<TMessage>, TReceiver>()
                .AddHostedService(services => services.GetService<QueueMessageReceiver<TMessage>>()!);
    }
}