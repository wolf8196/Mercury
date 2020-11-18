using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Mercury.Utils
{
    public static class LoggingExtensions
    {
        private const string ErrorMetadata = "@Error";

        public static IDisposable BeginScope(this ILogger logger)
        {
            return logger.BeginScope<object>(null);
        }

        public static IFluentScopeLogger WithScope(this ILogger logger, string propertyName, object value)
        {
            if (logger is IFluentScopeLogger scopeLogger)
            {
                scopeLogger.WithScope(propertyName, value);
            }
            else
            {
                scopeLogger = new FluentScopeLogger(logger).WithScope(propertyName, value);
            }

            return scopeLogger;
        }

        public static ILogger WithErrorScope(this ILogger logger, IEnumerable<object> errors)
        {
            return logger.WithScope(ErrorMetadata, errors);
        }

        public static ILogger WithErrorScope(this ILogger logger, object error)
        {
            return logger.WithScope(ErrorMetadata, error);
        }
    }
}