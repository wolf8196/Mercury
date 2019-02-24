using System.Collections.Generic;

namespace Mercury.Abstraction.Interfaces
{
    public interface ITemplateProcessor
    {
        string Process(string template, IDictionary<string, object> model);
    }
}