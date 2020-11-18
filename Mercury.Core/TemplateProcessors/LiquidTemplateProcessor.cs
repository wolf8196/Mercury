using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using FluentResults;
using Mercury.Utils;
using Microsoft.Extensions.Logging;

namespace Mercury.Core.TemplateProcessors
{
    public class LiquidTemplateProcessor : TemplateProcessor
    {
        public LiquidTemplateProcessor(ILogger<LiquidTemplateProcessor> logger)
            : base(logger)
        {
        }

        public override Result<string> Process(string template, IDictionary<string, object> model)
        {
            var parsedTemplate = Template.Parse(template);

            var renderedTemplate = parsedTemplate.Render(Hash.FromDictionary(model));

            if (parsedTemplate.Errors.Any())
            {
                var errors = parsedTemplate.Errors.Select(x => x.Message);
                Logger.WithErrorScope(errors).LogError(TemplateProcessingFailureMessage);
                return Result.Fail(new Error(TemplateProcessingFailureMessage).CausedBy(errors));
            }

            return Result.Ok(renderedTemplate);
        }
    }
}