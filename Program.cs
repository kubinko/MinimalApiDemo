using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AttendanceDb>(options => options.UseInMemoryDatabase("attendancedb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

var group = app.MapGroup("attendance");

group.MapPost("/", () => "New attendee added.");
group.MapPut("{id}", (long id) => $"Attendee {id} updated.");
group.MapGet("{id}", (long id) => $"Attendee {id}.");
group.MapGet("", () => "List of all attendees.");
group.MapDelete("{id}", (long id) => $"Attendee {id} deleted.");

app.Run();

record Attendee(string Name, int YearOfBirth, string Email, string Phone)
{
    public long Id { get; set; }
};

class AttendanceDb : DbContext
{
    public AttendanceDb(DbContextOptions<AttendanceDb> context) : base(context)
    {

    }

    public DbSet<Attendee> Attendees { get; set; } = default!;
}