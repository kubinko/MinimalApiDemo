namespace MinimalApi.Messaging.Messages
{
    public record InvoiceGeneratedMessage(
        long AttendeeId,
        string AttendeeName,
        string AttendeeEmail,
        string InvoiceCode,
        string WorkshopName,
        decimal Price);
}
