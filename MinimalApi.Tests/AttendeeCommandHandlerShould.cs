using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Database;
using MinimalApi.Queries;

namespace MinimalApi.Tests
{
    public class AttendeeQueryHandlerShould
    {
        [Fact]
        public async Task ReturnNotFoundResponseIfAttendeeWithSpecifiedIdDoesNotExist()
        {
            var query = new SingleAttendeeQuery(42);
            var handler = CreateHandler();

            IResult result = await handler.Handle(query, default);

            result.Should().BeOfType<NotFound>();
        }

        [Fact]
        public async Task ReturnAttendeeWithSpecifiedId()
        {
            var attendee = new Attendee("Bill Gates", 1955, "gates@codera.ma", "") { Id = 42 };
            var query = new SingleAttendeeQuery(42);
            var handler = CreateHandler(attendee);

            IResult result = await handler.Handle(query, default);

            result.Should().BeOfType<Ok<Attendee>>();
            (result as Ok<Attendee>)!.Value.Should().Be(attendee);
        }

        private static AttendeeQueryHandler CreateHandler(Attendee? seededAttendee = null)
        {
            var db = new AttendanceDb(new DbContextOptionsBuilder<AttendanceDb>().UseInMemoryDatabase("testdb").Options);
            if (seededAttendee != null)
            {
                db.Add(seededAttendee);
                db.SaveChanges();
            }

            return new(db);
        }

    }
}