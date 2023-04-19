using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MinimalApi.Invoicing.Services;
using MinimalApi.Messaging.Messages;
using System.Text.Json;

namespace MinimalApi.Invoicing.Functions
{
    public class InvoiceGeneratorFunction
    {
        private readonly InvoicingService _invoicingService;
        private readonly ILogger _logger;

        public InvoiceGeneratorFunction(ILoggerFactory loggerFactory)
        {
            _invoicingService = new InvoicingService(loggerFactory);
            _logger = loggerFactory.CreateLogger<InvoiceGeneratorFunction>();
        }

        [Function(nameof(InvoiceGeneratorFunction))]
        [ServiceBusOutput(TopicNames.InvoiceGeneratedTopic, ServiceBusEntityType.Topic, Connection = "ServiceBus:ConnectionString")]
        public async Task<string> Run([ServiceBusTrigger(
            TopicNames.AttendeeRegistrationTopic,
            "%ServiceBus:SubscriptionRegistration%",
            Connection = "ServiceBus:ConnectionString")] string message,
            CancellationToken cancellationToken)
        {
            var payload = JsonSerializer.Deserialize<AttendeeRegistrationMessage>(message);
            string invoiceCode = await _invoicingService.GenerateAndSaveInvoice(payload!, cancellationToken);

            _logger.LogInformation($"Generated invoice with code {invoiceCode}.");

            return JsonSerializer.Serialize(new InvoiceGeneratedMessage(
                payload!.AttendeeId,
                payload!.Name,
                payload!.Email,
                invoiceCode));
        }
    }
}
