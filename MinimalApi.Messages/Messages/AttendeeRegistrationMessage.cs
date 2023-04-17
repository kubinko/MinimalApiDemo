namespace MinimalApi.Messaging.Messages
{
    public record AttendeeRegistrationMessage(
        long AttendeeId,
        string Name,
        string Email,
        string WorkshopName,
        decimal WorkshopPrice);
}