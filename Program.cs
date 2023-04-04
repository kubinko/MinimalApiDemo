using Microsoft.EntityFrameworkCore;
using MinimalApi.Database;
using MinimalApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AttendanceDb>(options => options.UseInMemoryDatabase("attendancedb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSingleton<IIdGenerator, InMemoryIdGenerator>();

var app = builder.Build();

var group = app.MapGroup("attendance");

group.MapPost("/", async (Attendee attendee, AttendanceDb db, IIdGenerator idgenerator) =>
{
    attendee.Id = idgenerator.GenerateAttendeeId();
    await db.AddAsync(attendee);
    await db.SaveChangesAsync();

    return Results.Created($"attendance/{attendee.Id}", new { attendee.Id });
});
group.MapPut("{id}", async (long id, Attendee attendee, AttendanceDb db) =>
{
    attendee.Id = id;
    db.Update(attendee);

    await db.SaveChangesAsync();

    return Results.NoContent();
});
group.MapGet("{id}", (long id, AttendanceDb db) =>
{
    var savedAttendee = db.Attendees.SingleOrDefault(att => att.Id == id);
    if (savedAttendee == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(savedAttendee);
    }
});
group.MapGet("", (AttendanceDb db) => Results.Ok(db.Attendees.ToArray()));
group.MapDelete("{id}", async (long id, AttendanceDb db) =>
{
    var savedAttendee = db.Attendees.SingleOrDefault(att => att.Id == id);
    if (savedAttendee != null)
    {
        db.Attendees.Remove(savedAttendee);
        await db.SaveChangesAsync();
    }

    return Results.NoContent();
});

app.Run();
