using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Database;

namespace MinimalApi.Queries
{
    public class AttendeeQueryHandler :
        IRequestHandler<SingleAttendeeQuery, IResult>,
        IRequestHandler<AllAttendeesQuery, IResult>
    {
        private readonly AttendanceDb _db;

        public AttendeeQueryHandler(AttendanceDb db)
        {
            _db = db;
        }

        public async Task<IResult> Handle(SingleAttendeeQuery request, CancellationToken cancellationToken)
        {
            var savedAttendee = await _db.Attendees.SingleOrDefaultAsync(att => att.Id == request.Id, cancellationToken);
            if (savedAttendee == null)
            {
                return Results.NotFound();
            }
            else
            {
                return Results.Ok(savedAttendee);
            }
        }

        public Task<IResult> Handle(AllAttendeesQuery request, CancellationToken cancellationToken)
            => Task.FromResult(Results.Ok(_db.Attendees.ToArray()));
    }
}
