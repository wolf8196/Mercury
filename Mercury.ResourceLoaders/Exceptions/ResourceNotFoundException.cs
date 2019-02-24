using System;
using Mercury.ResourceLoaders.Enums;

namespace Mercury.ResourceLoaders.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string path, ResourceLoaderTypes type)
            : this("Requested resource was not found", path, type)
        {
        }

        public ResourceNotFoundException(string msg, string path, ResourceLoaderTypes type)
            : base(msg)
        {
            Path = path;
            ResourceLoaderType = type;
        }

        public string Path { get; set; }

        public ResourceLoaderTypes ResourceLoaderType { get; set; }
    }
}