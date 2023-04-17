using MediatR;
using MinimalApi.Notifications.Services;

namespace MinimalApi.Notifications.Queries
{
    public class NotificationsLogQueryHandler : IRequestHandler<NotificationsLogQuery, IEnumerable<string>>
    {
        private readonly INotificationService _notificationService;

        public NotificationsLogQueryHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public Task<IEnumerable<string>> Handle(NotificationsLogQuery request, CancellationToken cancellationToken)
            => Task.FromResult(_notificationService.GetNotificationLog());
    }
}
