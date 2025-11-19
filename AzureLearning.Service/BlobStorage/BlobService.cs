using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Azure.Security.KeyVault.Secrets;
using AzureLearning.Model.Settings;
using Microsoft.Extensions.Configuration;

namespace AzureLearning.Service.BlobStorage
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _container;
        private readonly string _accountName;
        private readonly string _accountKey;


        public BlobService(IConfiguration config)
        {
            var settings = config.GetSection("KeyVault").Get<BlobSettings>();
            string _blobConnectionString = null;
            string _blobContainerName = null;

            if (!string.IsNullOrWhiteSpace(settings?.VaultUri))
            {
                var secretClient = new SecretClient(new Uri(settings.VaultUri), new DefaultAzureCredential());

                var connSecret = secretClient.GetSecret(settings.BlobConnectionStringSecretName);
                _blobConnectionString = connSecret.Value.Value;

                var containerSecret = secretClient.GetSecret(settings.BlobContainerNameSecretName);
                _blobContainerName = containerSecret.Value.Value;
            }

            var client = new BlobServiceClient(_blobConnectionString);
            _container = client.GetBlobContainerClient(_blobContainerName);
            _container.CreateIfNotExists(PublicAccessType.None);

            _accountName = ParseConnectionStringValue(_blobConnectionString, "AccountName");
            _accountKey = ParseConnectionStringValue(_blobConnectionString, "AccountKey");
        }


        public async Task<IEnumerable<string>> ListBlobsAsync()
        {
            var items = new List<string>();
            await foreach (var blob in _container.GetBlobsAsync())
            {
                items.Add(blob.Name);
            }
            return items;
        }


        public async Task UploadBlobAsync(string name, Stream data, string contentType)
        {
            var blobClient = _container.GetBlobClient(name);
            await blobClient.UploadAsync(data, new BlobHttpHeaders { ContentType = contentType });
        }


        public async Task<Stream> DownloadBlobAsync(string name)
        {
            var blob = _container.GetBlobClient(name);
            var resp = await blob.DownloadAsync();
            return resp.Value.Content;
        }


        public async Task DeleteBlobAsync(string name)
        {
            var blob = _container.GetBlobClient(name);
            await blob.DeleteIfExistsAsync();
        }

        public Task<string> GetBlobSasUrlAsync(string name, int expiryMinutes = 60)
        {
            var blobClient = _container.GetBlobClient(name);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _container.Name,
                BlobName = name,
                Resource = "b", // b = blob
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // allow for clock skew
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
            };

            // Allow only read (download) permission
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var credential = new StorageSharedKeyCredential(_accountName, _accountKey);
            var sasQueryParameters = sasBuilder.ToSasQueryParameters(credential);
            var sasToken = sasQueryParameters.ToString();

            var sasUri = $"{blobClient.Uri}?{sasToken}";
            return Task.FromResult(sasUri);
        }

        private static string ParseConnectionStringValue(string connectionString, string key)
        {
            var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (part.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                    return part.Substring(key.Length + 1);
            }
            return null;
        }
    }
}
