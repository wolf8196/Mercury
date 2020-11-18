using Microsoft.Extensions.Logging;

namespace Mercury.Utils
{
    public interface IFluentScopeLogger : ILogger
    {
        IFluentScopeLogger WithScope(string propertyName, object value);
    }
}