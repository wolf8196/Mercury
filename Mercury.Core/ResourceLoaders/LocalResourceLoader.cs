using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Mercury.Utils;
using Microsoft.Extensions.Logging;

namespace Mercury.Core.ResourceLoaders
{
    public class LocalResourceLoader : ResourceLoader
    {
        private readonly LocalResourceSettings settings;

        public LocalResourceLoader(ILogger<LocalResourceLoader> logger, LocalResourceSettings settings)
            : base(logger)
        {
            this.settings = settings;

            settings.ThrowIfNull(nameof(settings));
            settings.RootPath.ThrowIfNull(nameof(settings.RootPath));
        }

        public override async Task<Result<string>> LoadAsync(string path, CancellationToken token)
        {
            var root = Path.Combine(settings.RootPath, Map(path));

            if (!File.Exists(root))
            {
                return NotExistResult(root);
            }

            using (var stream = new FileStream(root, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                return await LoadAsync(stream, token).ConfigureAwait(false);
            }
        }

        private string Map(string path)
        {
            return path.Replace("/", "\\");
        }
    }
}