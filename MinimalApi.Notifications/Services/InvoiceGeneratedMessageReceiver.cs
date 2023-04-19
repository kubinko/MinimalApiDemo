using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using MinimalApi.Common.Options;
using MinimalApi.Messages;
using MinimalApi.Messaging.Messages;
using MinimalApi.Messaging.Services;
using System.Text.Json;

namespace MinimalApi.Notifications.Services
{
    public class InvoiceGeneratedMessageReceiver : TopicMessageReceiver<InvoiceGeneratedMessage>
    {
        private readonly INotificationService _notificationService;
        private readonly WorkshopSettings _workshopSettings;

        public InvoiceGeneratedMessageReceiver(
            INotificationService notificationService,
            IOptions<ServiceBusOptions> options,
            IOptions<WorkshopSettings> workshopSettings,
            ILogger<InvoiceGeneratedMessageReceiver> logger)
            : base(options, "invoiceGenerated", logger)
        {
            _notificationService = notificationService;
            _workshopSettings = workshopSettings?.Value ?? throw new ArgumentNullException(nameof(workshopSettings));
        }

        protected override async Task ProcessMessage(ProcessMessageEventArgs e)
        {
            _logger.LogInformation($"Received invoice generated message: {e.Message?.Body?.ToString()}");

            if (e.Message?.Body != null)
            {
                var message = JsonSerializer.Deserialize<InvoiceGeneratedMessage>(e.Message.Body);
                if (message != null)
                {
                    _notificationService.SendNotification(
                        message.AttendeeName,
                        message.AttendeeEmail,
                        $"Invoice {message.InvoiceCode} for workshop {_workshopSettings.Name}",
                        $"Invoice for workshop {_workshopSettings.Name} has been generated with price {_workshopSettings.Price:n2} EUR.");
                }
            }

            await e.CompleteMessageAsync(e.Message, e.CancellationToken);
        }
    }
}
