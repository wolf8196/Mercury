namespace Mercury.Abstraction.Interfaces
{
    public interface IPathFinder
    {
        string GetTemplatePath(string key);

        string GetMetadataPath(string key);
    }
}