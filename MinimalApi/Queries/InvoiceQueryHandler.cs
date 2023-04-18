using MediatR;
using Microsoft.Extensions.Options;
using MinimalApi.Database;
using MinimalApi.Options;

namespace MinimalApi.Queries
{
    public class InvoiceQueryHandler : IRequestHandler<InvoiceQuery, IResult>
    {
        private readonly AttendanceDb _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly InvoicingSettings _settings;

        public InvoiceQueryHandler(AttendanceDb db, IHttpClientFactory httpClientFactory, IOptions<InvoicingSettings> settings)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<IResult> Handle(InvoiceQuery request, CancellationToken cancellationToken)
        {
            var attendee = await _db.Attendees.FindAsync(new object[] { request.AttendeeId }, cancellationToken);
            if (attendee == null || string.IsNullOrEmpty(attendee.InvoiceCode))
            {
                return Results.NotFound();
            }
            else if (!attendee.InvoiceCode.Equals(request.Code))
            {
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            }

            using HttpClient client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("x-functions-key", _settings.ApiKey);
            HttpResponseMessage response =
                await client.GetAsync(new Uri($"{_settings.InvoicingUri}?code={request.Code}"), cancellationToken);

            if (response != null && response.IsSuccessStatusCode)
            {
                var url = await response.Content.ReadAsStringAsync(cancellationToken);
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    return Results.Redirect(url);
                }
            }

            return Results.NotFound();
        }
    }
}
