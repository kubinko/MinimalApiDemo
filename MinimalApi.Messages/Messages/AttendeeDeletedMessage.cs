namespace MinimalApi.Messaging.Messages
{
    public record AttendeeDeletedMessage(string Name, string Email, string WorkshopName);
}
