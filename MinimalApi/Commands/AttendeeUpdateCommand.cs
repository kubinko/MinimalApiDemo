using MediatR;
using MinimalApi.Database;

namespace MinimalApi.Commands
{
    public record AttendeeUpdateCommand(Attendee Attendee) : IRequest<IResult>;
}
