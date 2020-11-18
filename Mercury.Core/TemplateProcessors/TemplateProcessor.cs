using System.Collections.Generic;
using FluentResults;
using Mercury.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace Mercury.Core.TemplateProcessors
{
    public abstract class TemplateProcessor : ITemplateProcessor
    {
        protected const string TemplateProcessingFailureMessage = "Failed to process template.";

        protected TemplateProcessor(ILogger logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; set; }

        public abstract Result<string> Process(string template, IDictionary<string, object> model);
    }
}