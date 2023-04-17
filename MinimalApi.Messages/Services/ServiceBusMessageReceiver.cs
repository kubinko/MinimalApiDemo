using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Messages;

namespace MinimalApi.Messaging.Services
{
    public abstract class ServiceBusMessageReceiver<T> : IHostedService, IAsyncDisposable
    {
        protected readonly ServiceBusAdministrationClient _serviceBusAdminClient;
        protected readonly ServiceBusClient _serviceBusClient;
        private ServiceBusProcessor? _serviceBusProcessor;
        protected readonly ILogger<ServiceBusMessageReceiver<T>> _logger;

        public ServiceBusMessageReceiver(IOptions<ServiceBusOptions> options, ILogger<ServiceBusMessageReceiver<T>> logger)
        {
            string serviceBusConnectionString = options?.Value?.ConnectionString
                ?? throw new ArgumentNullException(nameof(options.Value.ConnectionString));
            _serviceBusAdminClient = new ServiceBusAdministrationClient(serviceBusConnectionString);
            _serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting service bus consumer.");

            try
            {
                _serviceBusProcessor = await CreateProcessor(cancellationToken);
                if (_serviceBusProcessor != null)
                {
                    _serviceBusProcessor.ProcessMessageAsync += ProcessMessage;
                    _serviceBusProcessor.ProcessErrorAsync += ProcessError;
                    await _serviceBusProcessor.StartProcessingAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating service bus processor: {ex.Message}");
            }
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stopping service bus consumer.");

            try
            {
                await DisposeProcessor();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error stopping service bus processor: {ex.Message}");
            }
        }

        protected abstract Task<ServiceBusProcessor> CreateProcessor(CancellationToken cancellationToken);

        protected abstract Task ProcessMessage(ProcessMessageEventArgs e);

        private Task ProcessError(ProcessErrorEventArgs e)
        {
            _logger.LogError($"Error processing message [{e.ErrorSource}]: {e.Exception}");
            return Task.CompletedTask;
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await DisposeProcessor();
            await _serviceBusClient.DisposeAsync();

            GC.SuppressFinalize(this);
        }

        private async Task DisposeProcessor()
        {
            if (_serviceBusProcessor != null)
            {
                if (_serviceBusProcessor.IsProcessing)
                {
                    await _serviceBusProcessor.StopProcessingAsync();
                }
                await _serviceBusProcessor.CloseAsync();
            }
        }
    }
}