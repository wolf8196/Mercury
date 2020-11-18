using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Mercury.Core.Abstractions;
using Mercury.Utils;
using Microsoft.Extensions.Logging;

namespace Mercury.Core.ResourceLoaders
{
    public abstract class ResourceLoader : IResourceLoader
    {
        private const string ResourceNotFoundFailureMessage = "Requested resource was not found.";
        private const string PathMetadata = "Path";

        protected ResourceLoader(ILogger logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; }

        public abstract Task<Result<string>> LoadAsync(string path, CancellationToken token);

        protected static async Task<Result<string>> LoadAsync(Stream stream, CancellationToken token)
        {
            using (var sr = new StreamReader(stream))
            {
                token.ThrowIfCancellationRequested();

                var resource = await sr.ReadToEndAsync().ConfigureAwait(false);
                return Result.Ok(resource);
            }
        }

        protected Result NotExistResult(string path)
        {
            Logger.WithScope(PathMetadata, path).LogError(ResourceNotFoundFailureMessage);
            return Result.Fail(new Error(ResourceNotFoundFailureMessage).WithMetadata(PathMetadata, path));
        }
    }
}