using Azure.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Commands;
using MinimalApi.Database;
using MinimalApi.Messages;
using MinimalApi.Messaging.Messages;
using MinimalApi.Messaging.Services;
using MinimalApi.Options;
using MinimalApi.Queries;
using MinimalApi.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

var appConfigConnectionString = builder.Configuration.GetConnectionString("AppConfig");
if (!string.IsNullOrEmpty(appConfigConnectionString))
{
    builder.Configuration.AddAzureAppConfiguration(options =>
        options.Connect(appConfigConnectionString)
            .ConfigureKeyVault(kv =>
            {
                kv.SetCredential(new DefaultAzureCredential());
            }));
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    Log.Information("Connected to in-memory database.");
    builder.Services
        .AddDbContext<AttendanceDb>(options => options.UseInMemoryDatabase("attendancedb"))
        .AddSingleton<IIdGenerator, InMemoryIdGenerator>();
}
else
{
    Log.Information("Connected to SQL database.");
    builder.Services
        .AddDbContext<AttendanceDb>(options => options.UseSqlServer(connectionString))
        .AddSingleton<IIdGenerator, FakeIdGenerator>();
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .Configure<ServiceBusOptions>(builder.Configuration.GetSection("ServiceBus"))
    .Configure<WorkshopSettings>(builder.Configuration.GetSection("Workshop"))
    .Configure<InvoicingSettings>(builder.Configuration.GetSection("Invoicing"));
builder.Services
    .AddScoped<IMessageSender, MessageSender>()
    .AddHttpClient();
builder.Services
    .AddSingleton<TopicMessageReceiver<InvoiceGeneratedMessage>, InvoiceGeneratedMessageReceiver>()
    .AddHostedService(services => services.GetService<TopicMessageReceiver<InvoiceGeneratedMessage>>()!);

var currentAssembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(currentAssembly));
builder.Services.AddValidatorsFromAssembly(currentAssembly);
builder.Services.AddHealthChecks();

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseStatusCodePages();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

var group = app.MapGroup("attendance");

group.MapPost("", async (IMediator mediator, Attendee attendee) => await mediator.Send(new AttendeeCreateCommand(attendee)));
group.MapGet("{id}", async (IMediator mediator, long id) => await mediator.Send(new SingleAttendeeQuery(id)));
group.MapGet("", async (IMediator mediator) => await mediator.Send(new AllAttendeesQuery()));
group.MapGet("{id}/invoice", async (IMediator mediator, long id, [FromQuery] string? code) => await mediator.Send(new InvoiceQuery(id, code)));
group.MapDelete("{id}", async (IMediator mediator, long id) => await mediator.Send(new AttendeeDeleteCommand() { Id = id }));

app.MapGet("/info", async (IMediator mediator) => await mediator.Send(new WorkshopInfoQuery()));
app.MapHealthChecks("/health");

app.Run();
