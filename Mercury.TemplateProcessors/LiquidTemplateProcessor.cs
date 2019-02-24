using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using Mercury.Abstraction.Interfaces;
using Mercury.TemplateProcessors.Exceptions;

namespace Mercury.TemplateProcessors
{
    public class LiquidTemplateProcessor : ITemplateProcessor
    {
        public string Process(string template, IDictionary<string, object> model)
        {
            var parsedTemplate = Template.Parse(template);

            var result = parsedTemplate.Render(Hash.FromDictionary(model));

            if (parsedTemplate.Errors.Any())
            {
                throw new TemplateProcessingException(Enums.TemplateProcessorTypes.Liquid, parsedTemplate.Errors.Select(x => x.Message).ToArray());
            }
            else
            {
                return result;
            }
        }
    }
}