using Microsoft.Extensions.Hosting;
using MinimalApi.Common;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        var appConfigConnectionString = Environment.GetEnvironmentVariable("AppConfig");
        builder.AddAzureAppConfigurationWithKeyVault(appConfigConnectionString);
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
