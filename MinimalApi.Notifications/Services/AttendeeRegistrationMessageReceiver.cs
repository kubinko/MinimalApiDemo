using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using MinimalApi.Common.Options;
using MinimalApi.Messages;
using MinimalApi.Messaging.Messages;
using MinimalApi.Messaging.Services;
using System.Text.Json;

namespace MinimalApi.Notifications.Services
{
    public class AttendeeRegistrationMessageReceiver : TopicMessageReceiver<AttendeeRegistrationMessage>
    {
        private readonly INotificationService _notificationService;
        private readonly WorkshopSettings _workshopSettings;

        public AttendeeRegistrationMessageReceiver(
            INotificationService notificationService,
            IOptions<ServiceBusOptions> options,
            IOptions<WorkshopSettings> workshopSettings,
            ILogger<AttendeeRegistrationMessageReceiver> logger)
            : base(options, "attendeeRegistration", logger)
        {
            _notificationService = notificationService;
            _workshopSettings = workshopSettings?.Value ?? throw new ArgumentNullException(nameof(workshopSettings));
        }

        protected override async Task ProcessMessage(ProcessMessageEventArgs e)
        {
            _logger.LogInformation($"Received attendee registration message: {e.Message?.Body?.ToString()}");

            if (e.Message?.Body != null)
            {
                var message = JsonSerializer.Deserialize<AttendeeRegistrationMessage>(e.Message.Body);
                if (message != null)
                {
                    _notificationService.SendNotification(
                        message.Name,
                        message.Email,
                        "Workshop registration",
                        $"You have been successfully registered to workshop {_workshopSettings.Name}.");
                }
            }

            await e.CompleteMessageAsync(e.Message, e.CancellationToken);
        }
    }
}
