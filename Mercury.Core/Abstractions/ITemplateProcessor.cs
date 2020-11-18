using System.Collections.Generic;
using FluentResults;

namespace Mercury.Core.Abstractions
{
    public interface ITemplateProcessor
    {
        Result<string> Process(string template, IDictionary<string, object> model);
    }
}