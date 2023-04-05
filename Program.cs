using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Commands;
using MinimalApi.Database;
using MinimalApi.Queries;
using MinimalApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AttendanceDb>(options => options.UseInMemoryDatabase("attendancedb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSingleton<IIdGenerator, InMemoryIdGenerator>();

var currentAssembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(currentAssembly));
builder.Services.AddValidatorsFromAssembly(currentAssembly);
builder.Services.AddHealthChecks();

var app = builder.Build();

var group = app.MapGroup("attendance");

group.MapPost("", async (IMediator mediator, Attendee attendee) => await mediator.Send(new AttendeeCreateCommand(attendee)));
group.MapPut("{id}", async (IMediator mediator, long id, Attendee attendee) =>
{
    attendee.Id = id;
    return await mediator.Send(new AttendeeUpdateCommand(attendee));
});
group.MapGet("{id}", async (IMediator mediator, long id) => await mediator.Send(new SingleAttendeeQuery(id)));
group.MapGet("", async (IMediator mediator) => await mediator.Send(new AllAttendeesQuery()));
group.MapDelete("{id}", async (IMediator mediator, long id) => await mediator.Send(new AttendeeDeleteCommand() { Id = id }));

app.MapHealthChecks("/health");

app.Run();
