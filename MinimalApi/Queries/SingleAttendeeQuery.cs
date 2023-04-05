using MediatR;

namespace MinimalApi.Queries
{
    public class SingleAttendeeQuery : IRequest<IResult>
    {
        public long Id { get; }

        public SingleAttendeeQuery(long id)
        {
            Id = id;
        }
    }
}
