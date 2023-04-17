namespace MinimalApi.Messaging.Messages
{
    public record InvoiceGeneratedMessage(long AttendeeId, string InvoiceCode);
}
