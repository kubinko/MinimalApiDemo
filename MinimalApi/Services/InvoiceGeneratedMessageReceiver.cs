using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using MinimalApi.Database;
using MinimalApi.Messages;
using MinimalApi.Messaging.Messages;
using MinimalApi.Messaging.Services;
using System.Text.Json;

namespace MinimalApi.Services
{
    public class InvoiceGeneratedMessageReceiver : TopicMessageReceiver<InvoiceGeneratedMessage>
    {
        private IServiceProvider _serviceProvider;

        public InvoiceGeneratedMessageReceiver(
            IServiceProvider serviceProvider,
            IOptions<ServiceBusOptions> options,
            ILogger<InvoiceGeneratedMessageReceiver> logger)
            : base(options, "invoiceGenerated", logger)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ProcessMessage(ProcessMessageEventArgs e)
        {
            _logger.LogInformation($"Received invoice generated message: {e.Message?.Body?.ToString()}");

            if (e.Message?.Body != null)
            {
                var message = JsonSerializer.Deserialize<InvoiceGeneratedMessage>(e.Message.Body);
                if (message != null)
                {
                    using IServiceScope scope = _serviceProvider.CreateScope();

                    AttendanceDb db = scope.ServiceProvider.GetRequiredService<AttendanceDb>();

                    var attendee = await db.Attendees.FindAsync(new object[] { message.AttendeeId }, e.CancellationToken);
                    if (attendee != null)
                    {
                        attendee.InvoiceCode = message.InvoiceCode;
                        await db.SaveChangesAsync(e.CancellationToken);
                    }
                }
            }

            await e.CompleteMessageAsync(e.Message, e.CancellationToken);
        }
    }
}
