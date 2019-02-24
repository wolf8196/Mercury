using System;
using System.IO;
using System.Threading.Tasks;
using Mercury.Abstraction.Interfaces;
using Mercury.ResourceLoaders.Exceptions;
using Mercury.ResourceLoaders.Settings;
using Mercury.Utils;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Mercury.ResourceLoaders
{
    public class AzureBlobResourceLoader : IResourceLoader
    {
        private readonly AzureBlobResourceSettings settings;
        private readonly CloudStorageAccount account;
        private readonly CloudBlobClient client;
        private readonly CloudBlobContainer container;

        public AzureBlobResourceLoader(IOptions<AzureBlobResourceSettings> options)
        {
            options.ThrowIfNull(nameof(options));

            settings = options.Value;

            settings.ThrowIfNull(nameof(settings));

            if (CloudStorageAccount.TryParse(settings.ConnectionString, out account))
            {
                client = account.CreateCloudBlobClient();
                container = client.GetContainerReference(settings.BlobContainerName);
            }
            else
            {
                throw new ArgumentException(nameof(settings.ConnectionString));
            }
        }

        public async Task<string> LoadAsync(string path)
        {
            var blobReference = container.GetBlobReference(path);

            if (!await blobReference.ExistsAsync())
            {
                throw new ResourceNotFoundException(path, Enums.ResourceLoaderTypes.AzureBlob);
            }

            using (var stream = await blobReference.OpenReadAsync().ConfigureAwait(false))
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}