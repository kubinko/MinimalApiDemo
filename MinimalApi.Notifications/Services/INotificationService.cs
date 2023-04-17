namespace MinimalApi.Notifications.Services
{
    public interface INotificationService
    {
        void SendNotification(string recipientName, string recipientEmail, string subject, string body);

        IEnumerable<string> GetNotificationLog();
    }
}
