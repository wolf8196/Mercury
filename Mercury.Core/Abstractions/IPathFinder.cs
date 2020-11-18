namespace Mercury.Core.Abstractions
{
    public interface IPathFinder
    {
        string GetTemplatePath(string key);

        string GetMetadataPath(string key);
    }
}