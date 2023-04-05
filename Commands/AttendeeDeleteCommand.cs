using MediatR;

namespace MinimalApi.Commands
{
    public record AttendeeDeleteCommand : IRequest<IResult>
    {
        public long Id { get; set; }
    }
}
