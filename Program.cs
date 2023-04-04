var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var group = app.MapGroup("attendance");

group.MapPost("/", () => "New attendee added.");
group.MapPut("{id}", (long id) => $"Attendee {id} updated.");
group.MapGet("{id}", (long id) => $"Attendee {id}.");
group.MapGet("", () => "List of all attendees.");
group.MapDelete("{id}", (long id) => $"Attendee {id} deleted.");

app.Run();
