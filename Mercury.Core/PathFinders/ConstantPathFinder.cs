using Mercury.Core.Abstractions;

namespace Mercury.Core.PathFinders
{
    public class ConstantPathFinder : IPathFinder
    {
        public const string TemplateFileName = "template.html";
        public const string MetadataFileName = "metadata.json";

        public string GetMetadataPath(string key)
        {
            return $"{key}/{MetadataFileName}";
        }

        public string GetTemplatePath(string key)
        {
            return $"{key}/{TemplateFileName}";
        }
    }
}