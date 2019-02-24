using System.Threading.Tasks;

namespace Mercury.Abstraction.Interfaces
{
    public interface IResourceLoader
    {
        Task<string> LoadAsync(string path);
    }
}