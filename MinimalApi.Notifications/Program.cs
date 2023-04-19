using MediatR;
using MinimalApi.Common;
using MinimalApi.Common.Options;
using MinimalApi.Messages;
using MinimalApi.Messaging.Messages;
using MinimalApi.Notifications.Queries;
using MinimalApi.Notifications.Services;

var builder = WebApplication.CreateBuilder(args);

var appConfigConnectionString = builder.Configuration.GetConnectionString("AppConfig");
builder.Configuration.AddAzureAppConfigurationWithKeyVault(appConfigConnectionString);

builder.Services.Configure<WorkshopSettings>(builder.Configuration.GetSection("Workshop"));

builder.Services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining<NotificationsLogQuery>());

builder.Services.Configure<ServiceBusOptions>(builder.Configuration.GetSection("ServiceBus"));
builder.Services
    .AddSingleton<INotificationService, LoggingNotificationService>()
    .AddTopicReceiver<AttendeeRegistrationMessage, AttendeeRegistrationMessageReceiver>()
    .AddTopicReceiver<InvoiceGeneratedMessage, InvoiceGeneratedMessageReceiver>()
    .AddQueueReceiver<AttendeeDeletedMessage, AttendeeDeletedMessageReceiver>();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapGet("/", async (IMediator mediator) => Results.Ok(await mediator.Send(new NotificationsLogQuery())));
app.MapHealthChecks("/health");

app.Run();
