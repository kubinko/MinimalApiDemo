using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace MinimalApi.Invoicing.Services
{
    internal class BlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService()
        {
            _connectionString = Environment.GetEnvironmentVariable("Storage:ConnectionString") ?? throw new ArgumentNullException("Connection string");
            _containerName = Environment.GetEnvironmentVariable("Storage:Container") ?? throw new ArgumentNullException("Container name");
        }

        public async Task SaveBlob(Stream data, string blobName, string contentType, CancellationToken cancellationToken)
        {
            var containerClient = await GetBlobContainerClient(cancellationToken);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(
                data,
                new BlobUploadOptions()
                {
                    HttpHeaders = new BlobHttpHeaders()
                    {
                        ContentType = contentType
                    }
                },
                cancellationToken);
        }

        public async Task<Uri?> GetBlobDownloadUri(string blobName, CancellationToken cancellationToken)
        {
            var containerClient = await GetBlobContainerClient(cancellationToken);
            var blobClient = containerClient.GetBlobClient(blobName);

            return await blobClient.ExistsAsync(cancellationToken)
                ? blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(10))
                : null;
        }

        public async Task DeleteBlob(string blobName, CancellationToken cancellationToken)
        {
            var containerClient = await GetBlobContainerClient(cancellationToken);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        private async Task<BlobContainerClient> GetBlobContainerClient(CancellationToken cancellationToken)
        {
            var containerClient = new BlobContainerClient(_connectionString, _containerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            return containerClient;
        }
    }
}
