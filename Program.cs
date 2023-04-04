var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("attendance", () => "New attendee added.");
app.MapPut("attendance/{id}", (long id) => $"Attendee {id} updated.");
app.MapGet("attendance/{id}", (long id) => $"Attendee {id}.");
app.MapGet("attendance", () => "List of all attendees.");
app.MapDelete("attendance/{id}", (long id) => $"Attendee {id} deleted.");

app.Run();
