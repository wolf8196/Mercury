using Mercury.Core.Emailers;
using Mercury.Core.ResourceLoaders;
using Mercury.Core.TemplateProcessors;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mercury.Service.Settings
{
    public class ServiceSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResourceLoaderTypes ResourceLoader { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TemplateProcessorTypes TemplateProcessor { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EmailerTypes Emailer { get; set; }

        public bool StartMessageWorker { get; set; }
    }
}