using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using MinimalApi.Messages;
using MinimalApi.Messaging.Messages;
using MinimalApi.Messaging.Services;
using System.Text.Json;

namespace MinimalApi.Notifications.Services
{
    public class InvoiceGeneratedMessageReceiver : TopicMessageReceiver<InvoiceGeneratedMessage>
    {
        private readonly INotificationService _notificationService;

        public InvoiceGeneratedMessageReceiver(
            INotificationService notificationService,
            IOptions<ServiceBusOptions> options,
            ILogger<InvoiceGeneratedMessageReceiver> logger)
            : base(options, "invoiceGenerated", logger)
        {
            _notificationService = notificationService;
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
                        $"Invoice {message.InvoiceCode} for workshop {message.WorkshopName}",
                        $"Invoice for workshop {message.WorkshopName} has been generated with price {message.Price:n2} EUR.");
                }
            }

            await e.CompleteMessageAsync(e.Message, e.CancellationToken);
        }
    }
}
