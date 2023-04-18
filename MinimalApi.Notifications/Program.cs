using Azure.Identity;
using MediatR;
using MinimalApi.Messages;
using MinimalApi.Messaging.Messages;
using MinimalApi.Messaging.Services;
using MinimalApi.Notifications.Queries;
using MinimalApi.Notifications.Services;

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

builder.Services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining<NotificationsLogQuery>());

builder.Services.Configure<ServiceBusOptions>(builder.Configuration.GetSection("ServiceBus"));
builder.Services
    .AddSingleton<INotificationService, LoggingNotificationService>()
    .AddSingleton<TopicMessageReceiver<AttendeeRegistrationMessage>, AttendeeRegistrationMessageReceiver>()
    .AddSingleton<TopicMessageReceiver<InvoiceGeneratedMessage>, InvoiceGeneratedMessageReceiver>()
    .AddSingleton<QueueMessageReceiver<AttendeeDeletedMessage>, AttendeeDeletedMessageReceiver>()
    .AddHostedService(services => services.GetService<TopicMessageReceiver<AttendeeRegistrationMessage>>()!)
    .AddHostedService(services => services.GetService<TopicMessageReceiver<InvoiceGeneratedMessage>>()!)
    .AddHostedService(services => services.GetService<QueueMessageReceiver<AttendeeDeletedMessage>>()!);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapGet("/", async (IMediator mediator) => Results.Ok(await mediator.Send(new NotificationsLogQuery())));
app.MapHealthChecks("/health");

app.Run();
