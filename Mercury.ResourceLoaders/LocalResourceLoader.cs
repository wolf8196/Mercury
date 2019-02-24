using System.IO;
using System.Threading.Tasks;
using Mercury.Abstraction.Interfaces;
using Mercury.ResourceLoaders.Enums;
using Mercury.ResourceLoaders.Exceptions;
using Mercury.ResourceLoaders.Settings;
using Mercury.Utils;
using Microsoft.Extensions.Options;

namespace Mercury.ResourceLoaders
{
    public class LocalResourceLoader : IResourceLoader
    {
        private readonly LocalResourceSettings settings;

        public LocalResourceLoader(IOptions<LocalResourceSettings> options)
        {
            settings = options.ThrowIfNull(nameof(options)).Value;
            settings.ThrowIfNull(nameof(settings));
            settings.RootPath.ThrowIfNull(nameof(settings.RootPath));
        }

        public async Task<string> LoadAsync(string path)
        {
            var root = Path.Combine(settings.RootPath, Map(path));

            if (!File.Exists(root))
            {
                throw new ResourceNotFoundException(path, ResourceLoaderTypes.Local);
            }

            using (var fs = new FileStream(root, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs))
            {
                return await sr.ReadToEndAsync();
            }
        }

        private string Map(string path)
        {
            return path.Replace("/", "\\");
        }
    }
}