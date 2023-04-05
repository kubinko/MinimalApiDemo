using MediatR;
using MinimalApi.Database;
using MinimalApi.Services;

namespace MinimalApi.Commands
{
    public class AttendeeCommandHandler :
        IRequestHandler<AttendeeCreateCommand, IResult>,
        IRequestHandler<AttendeeUpdateCommand, IResult>,
        IRequestHandler<AttendeeDeleteCommand, IResult>
    {
        private readonly AttendanceDb _db;
        private readonly IIdGenerator _idGenerator;

        public AttendeeCommandHandler(AttendanceDb db, IIdGenerator idGenerator)
        {
            _db = db;
            _idGenerator = idGenerator;
        }

        public async Task<IResult> Handle(AttendeeCreateCommand request, CancellationToken cancellationToken)
        {
            request.Attendee.Id = _idGenerator.GenerateAttendeeId();

            await _db.AddAsync(request.Attendee, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return Results.Created($"attendance/{request.Attendee.Id}", new { request.Attendee.Id });
        }

        public async Task<IResult> Handle(AttendeeUpdateCommand request, CancellationToken cancellationToken)
        {
            _db.Update(request.Attendee);

            await _db.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        }

        public async Task<IResult> Handle(AttendeeDeleteCommand request, CancellationToken cancellationToken)
        {
            var savedAttendee = _db.Attendees.SingleOrDefault(att => att.Id == request.Id);
            if (savedAttendee != null)
            {
                _db.Attendees.Remove(savedAttendee);
                await _db.SaveChangesAsync(cancellationToken);
            }

            return Results.NoContent();
        }
    }
}
