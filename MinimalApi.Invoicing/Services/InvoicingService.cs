using CorePDF;
using CorePDF.Contents;
using CorePDF.Pages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Messages;
using MinimalApi.Messaging.Messages;
using MinimalApi.Messaging.Services;

namespace MinimalApi.Invoicing.Services
{
    internal class InvoicingService
    {
        private readonly BlobStorageService _blobStorageService = new();
        private readonly MessageSender _messageSender;

        public InvoicingService(ILoggerFactory loggerFactory)
        {
            _messageSender = new MessageSender(
                Options.Create(new ServiceBusOptions()
                {
                    ConnectionString = Environment.GetEnvironmentVariable("ServiceBus:ConnectionString")
                        ?? throw new ArgumentNullException("Connection string")
                }),
                loggerFactory.CreateLogger<MessageSender>());
        }

        public async Task<string> GenerateAndSaveInvoice(
            AttendeeRegistrationMessage information,
            CancellationToken cancellationToken)
        {
            string invoiceCode = Guid.NewGuid().ToString();

            var pdf = new Document();
            pdf.Pages.Add(new Page
            {
                PageSize = Paper.PAGEA4PORTRAIT,
                Contents = new List<Content>
                {
                    new TextBox
                    {
                        Text =
                            $"INVOICE {invoiceCode}\n" +
                            $"Recipient: {information.Name} ({information.Email})\n" +
                            $"Subject: {information.WorkshopName}\n" +
                            $"Price: {information.WorkshopPrice:n2} EUR",
                        Position = PositionAnchor.Top,
                        PosX = 79,
                        PosY = 763
                    }
                }
            }); ;

            using var stream = new MemoryStream();
            pdf.Publish(stream);

            await _blobStorageService.SaveBlob(
                new MemoryStream(stream.ToArray()), GetInvoiceFileName(invoiceCode), "application/pdf", cancellationToken);

            await _messageSender.SendMessageToTopic(
                TopicNames.InvoiceGeneratedTopic,
                new InvoiceGeneratedMessage(
                    information.AttendeeId,
                    information.Name,
                    information.Email,
                    invoiceCode,
                    information.WorkshopName,
                    information.WorkshopPrice),
                cancellationToken);

            return invoiceCode;
        }

        public Task<Uri?> GetInvoiceUri(string invoiceCode, CancellationToken cancellationToken)
            => _blobStorageService.GetBlobDownloadUri(GetInvoiceFileName(invoiceCode), cancellationToken);

        public Task DeleteInvoice(string invoiceCode, CancellationToken cancellationToken)
            => _blobStorageService.DeleteBlob(GetInvoiceFileName(invoiceCode), cancellationToken);

        private static string GetInvoiceFileName(string invoiceCode)
            => $"{invoiceCode}.pdf";
    }
}
