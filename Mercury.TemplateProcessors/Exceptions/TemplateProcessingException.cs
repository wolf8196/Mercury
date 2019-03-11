using System;
using System.Collections.Generic;
using Mercury.TemplateProcessors.Enums;

namespace Mercury.TemplateProcessors.Exceptions
{
    public class TemplateProcessingException : Exception
    {
        public TemplateProcessingException(TemplateProcessorTypes type, params string[] errors)
            : this("Failed to process template", type, errors)
        {
        }

        public TemplateProcessingException(string msg, TemplateProcessorTypes type, IEnumerable<string> errors)
        : base(msg)
        {
            TemplateProcessorType = type;
            Errors = errors;
        }

        public IEnumerable<string> Errors { get; set; }

        public TemplateProcessorTypes TemplateProcessorType { get; set; }

        public override string Message
        {
            get
            {
                return $@"{base.Message}.
TemplateProcessor type: {TemplateProcessorType.ToString()}.
Errors:
{string.Join(Environment.NewLine, Errors)}";
            }
        }
    }
}