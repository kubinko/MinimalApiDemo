using MediatR;

namespace MinimalApi.Notifications.Queries
{
    public class NotificationsLogQuery : IRequest<IEnumerable<string>>
    {
    }
}
