using MediatR;
using MinimalApi.Database;

namespace MinimalApi.Commands
{
    public record AttendeeCreateCommand(Attendee Attendee) : IRequest<IResult>
    {
        public long Id { get; set; }
    };
}
