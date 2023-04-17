using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MinimalApi.Invoicing.Services;
using MinimalApi.Messaging.Messages;
using System.Text.Json;

namespace MinimalApi.Invoicing.Functions
{
    public class InvoiceDeleteFunction
    {
        private readonly InvoicingService _invoicingService;
        private readonly ILogger _logger;

        public InvoiceDeleteFunction(ILoggerFactory loggerFactory)
        {
            _invoicingService = new InvoicingService(loggerFactory);
            _logger = loggerFactory.CreateLogger<InvoiceDeleteFunction>();
        }

        [Function(nameof(InvoiceDeleteFunction))]
        public async Task Run(
            [ServiceBusTrigger(QueueNames.InvoiceDeleteQueue, Connection = "ServiceBus:ConnectionString")] string message,
            CancellationToken cancellationToken)
        {
            var payload = JsonSerializer.Deserialize<InvoiceDeleteMessage>(message);
            if (payload == null)
            {
                _logger.LogError($"Invalid invoice delete message payload: {message}.");
            }
            else
            {
                await _invoicingService.DeleteInvoice(payload.InvoiceCode, cancellationToken);
                _logger.LogInformation($"Deleted invoice with code {payload.InvoiceCode}.");
            }
        }
    }
}
