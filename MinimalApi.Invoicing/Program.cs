using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        var appConfigConnectionString = Environment.GetEnvironmentVariable("AppConfig");
        if (!string.IsNullOrEmpty(appConfigConnectionString))
        {
            builder.AddAzureAppConfiguration(options =>
                options.Connect(appConfigConnectionString)
                    .ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(new DefaultAzureCredential());
                    }));
        }
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
