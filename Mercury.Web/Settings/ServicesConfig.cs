using Mercury.Emailers.Enums;
using Mercury.ResourceLoaders.Enums;
using Mercury.TemplateProcessors.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mercury.Web.Settings
{
    public class ServicesConfig
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResourceLoaderTypes ResourceLoader { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TemplateProcessorTypes TemplateProcessor { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EmailerTypes Emailer { get; set; }
    }
}