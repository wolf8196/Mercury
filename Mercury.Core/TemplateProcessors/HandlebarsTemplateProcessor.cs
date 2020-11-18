using System;
using System.Collections.Generic;
using FluentResults;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;

namespace Mercury.Core.TemplateProcessors
{
    public class HandlebarsTemplateProcessor : TemplateProcessor
    {
        public HandlebarsTemplateProcessor(ILogger<HandlebarsTemplateProcessor> logger)
            : base(logger)
        {
        }

        public override Result<string> Process(string template, IDictionary<string, object> model)
        {
            try
            {
                var compiledTemplate = Handlebars.Compile(template);

                var rendered = compiledTemplate(model);

                return Result.Ok(rendered);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, TemplateProcessingFailureMessage);
                return Result.Fail(TemplateProcessingFailureMessage);
            }
        }
    }
}