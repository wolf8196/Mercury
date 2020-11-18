using System.Threading;
using System.Threading.Tasks;
using FluentResults;

namespace Mercury.Core.Abstractions
{
    public interface IResourceLoader
    {
        Task<Result<string>> LoadAsync(string path, CancellationToken token);
    }
}