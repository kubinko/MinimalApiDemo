using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MinimalApi.Invoicing.Services;
using System.Net;

namespace MinimalApi.Invoicing.Functions
{
    public class InvoiceUriProvider
    {
        private readonly InvoicingService _invoicingService;
        private readonly ILogger _logger;

        public InvoiceUriProvider(ILoggerFactory loggerFactory)
        {
            _invoicingService = new InvoicingService(loggerFactory);
            _logger = loggerFactory.CreateLogger<InvoiceUriProvider>();
        }

        [Function(nameof(InvoiceUriProvider))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
            CancellationToken cancellationToken)
        {
            var invoiceCode = req.Query.GetValues("code")?.FirstOrDefault();
            if (string.IsNullOrEmpty(invoiceCode))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var invoiceUri = await _invoicingService.GetInvoiceUri(invoiceCode, cancellationToken);
            if (invoiceUri == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }


            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString(invoiceUri.ToString());

            return response;
        }
    }
}
