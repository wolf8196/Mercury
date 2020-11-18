using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FluentResults;
using Mercury.Utils;
using Microsoft.Extensions.Logging;

namespace Mercury.Core.ResourceLoaders
{
    public class AzureBlobResourceLoader : ResourceLoader
    {
        private readonly BlobContainerClient containerClient;

        public AzureBlobResourceLoader(ILogger<AzureBlobResourceLoader> logger, AzureBlobResourceSettings settings)
            : base(logger)
        {
            settings.ThrowIfNull(nameof(settings));
            settings.ConnectionString.ThrowIfNullOrEmpty(nameof(settings.ConnectionString));
            settings.BlobContainerName.ThrowIfNullOrEmpty(nameof(settings.BlobContainerName));

            containerClient = new BlobServiceClient(settings.ConnectionString)
                .GetBlobContainerClient(settings.BlobContainerName);
        }

        public override async Task<Result<string>> LoadAsync(string path, CancellationToken token)
        {
            var blobClient = containerClient.GetBlobClient(path);

            if (!await blobClient.ExistsAsync(token).ConfigureAwait(false))
            {
                return NotExistResult(path);
            }

            using (var stream = await blobClient.OpenReadAsync(cancellationToken: token).ConfigureAwait(false))
            {
                return await LoadAsync(stream, token).ConfigureAwait(false);
            }
        }
    }
}