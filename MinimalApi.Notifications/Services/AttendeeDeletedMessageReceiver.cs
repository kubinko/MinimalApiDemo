using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using MinimalApi.Common.Options;
using MinimalApi.Messages;
using MinimalApi.Messaging.Messages;
using MinimalApi.Messaging.Services;
using System.Text.Json;

namespace MinimalApi.Notifications.Services
{
    public class AttendeeDeletedMessageReceiver : QueueMessageReceiver<AttendeeDeletedMessage>
    {
        private readonly INotificationService _notificationService;
        private readonly WorkshopSettings _workshopSettings;

        public AttendeeDeletedMessageReceiver(
            INotificationService notificationService,
            IOptions<ServiceBusOptions> options,
            IOptions<WorkshopSettings> workshopSettings,
            ILogger<AttendeeDeletedMessageReceiver> logger)
            : base(options, "attendeeDelete", logger)
        {
            _notificationService = notificationService;
            _workshopSettings = workshopSettings?.Value ?? throw new ArgumentNullException(nameof(workshopSettings));
        }

        protected override async Task ProcessMessage(ProcessMessageEventArgs e)
        {
            _logger.LogInformation($"Received attendee delete message: {e.Message?.Body?.ToString()}");

            if (e.Message?.Body != null)
            {
                var message = JsonSerializer.Deserialize<AttendeeDeletedMessage>(e.Message.Body);
                if (message != null)
                {
                    _notificationService.SendNotification(
                        message.Name,
                        message.Email,
                        "Registration cancelled",
                        $"Your registration to workshop {_workshopSettings.Name} has been cancelled.");
                }
            }

            await e.CompleteMessageAsync(e.Message, e.CancellationToken);
        }
    }
}
