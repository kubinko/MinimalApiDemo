using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Options;
using MinimalApi.Common.Options;
using MinimalApi.Database;
using MinimalApi.Messaging.Messages;
using MinimalApi.Messaging.Services;
using MinimalApi.Services;

namespace MinimalApi.Commands
{
    public class AttendeeCommandHandler :
        IRequestHandler<AttendeeCreateCommand, IResult>,
        IRequestHandler<AttendeeDeleteCommand, IResult>
    {
        private readonly AttendanceDb _db;
        private readonly IIdGenerator _idGenerator;
        private readonly IValidator<Attendee> _validator;
        private readonly IMessageSender _messageSender;
        private readonly WorkshopSettings _settings;

        public AttendeeCommandHandler(
            AttendanceDb db,
            IIdGenerator idGenerator,
            IValidator<Attendee> validator,
            IMessageSender messageSender,
            IOptions<WorkshopSettings> settings)
        {
            _db = db;
            _idGenerator = idGenerator;
            _validator = validator;
            _messageSender = messageSender;
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<IResult> Handle(AttendeeCreateCommand request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request.Attendee);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.ToDictionary());
            }

            request.Attendee.Id = _idGenerator.GenerateAttendeeId();

            await _db.AddAsync(request.Attendee, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await _messageSender.SendMessageToTopic(
                TopicNames.AttendeeRegistrationTopic,
                new AttendeeRegistrationMessage(
                    request.Attendee.Id,
                    request.Attendee.Name,
                    request.Attendee.Email,
                    _settings.Name,
                    _settings.Price),
                cancellationToken);

            return Results.Created($"attendance/{request.Attendee.Id}", new { request.Attendee.Id });
        }

        public async Task<IResult> Handle(AttendeeDeleteCommand request, CancellationToken cancellationToken)
        {
            var savedAttendee = _db.Attendees.SingleOrDefault(att => att.Id == request.Id);
            if (savedAttendee != null)
            {
                _db.Attendees.Remove(savedAttendee);
                await _db.SaveChangesAsync(cancellationToken);

                await _messageSender.SendMessageToQueue(
                    QueueNames.AttendeeDeleteQueue,
                    new AttendeeDeletedMessage(savedAttendee.Name, savedAttendee.Email),
                    cancellationToken);

                await _messageSender.SendMessageToQueue(
                    QueueNames.InvoiceDeleteQueue,
                    new InvoiceDeleteMessage(savedAttendee.InvoiceCode),
                    cancellationToken);
            }

            return Results.NoContent();
        }
    }
}
