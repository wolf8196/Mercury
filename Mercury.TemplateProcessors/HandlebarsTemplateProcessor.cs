using System;
using System.Collections.Generic;
using HandlebarsDotNet;
using Mercury.Abstraction.Interfaces;
using Mercury.TemplateProcessors.Exceptions;

namespace Mercury.TemplateProcessors
{
    public class HandlebarsTemplateProcessor : ITemplateProcessor
    {
        public string Process(string template, IDictionary<string, object> model)
        {
            try
            {
                var compiledTemplate = Handlebars.Compile(template);

                var rendered = compiledTemplate(model);

                return rendered;
            }
            catch (Exception ex)
            {
                throw new TemplateProcessingException(Enums.TemplateProcessorTypes.Handlebars, ex.Message);
            }
        }
    }
}