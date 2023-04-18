using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        var appConfigConnectionString = Environment.GetEnvironmentVariable("AppConfig");
        if (!string.IsNullOrEmpty(appConfigConnectionString))
        {
            builder.AddAzureAppConfiguration(appConfigConnectionString);
        }
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
