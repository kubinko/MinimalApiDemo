namespace MinimalApi.Notifications.Services
{
    public class LoggingNotificationService : INotificationService
    {
        private readonly static List<string> _notifications = new();

        public IEnumerable<string> GetNotificationLog()
            => _notifications;

        public void SendNotification(string recipientName, string recipientEmail, string subject, string body)
        {
            _notifications.Add($"[{DateTimeOffset.UtcNow:dd.MM.yyyy HH:mm:ss}] To: {recipientName} ({recipientEmail}) [{subject}] {body}");
        }
    }
}
